/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.WebAPI.Controllers;

using System;
using System.IO;
using System.Threading.Tasks;

using CNCLib.GCode.Generate;
using CNCLib.GCode.Generate.Load;
using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Shared;

public class GCodeLoadHelper
{
    private readonly IUserFileManager   _fileManager;
    private readonly ICNCLibUserContext _userContext;

    public GCodeLoadHelper(IUserFileManager fileManager, ICNCLibUserContext userContext)
    {
        _fileManager = fileManager;
        _userContext = userContext;
    }

    public async Task<LoadBase> CallLoad(LoadOptions opt, bool writeIntermediateFiles)
    {
        var loadBase = (opt.FileContent == null) ? CallLoadWithFileName(opt) : CallLoadWithContent(opt);

        if (writeIntermediateFiles && !string.IsNullOrEmpty(opt.GCodeWriteToFileName))
        {
            await CallWriteToDb("render.nc",   loadBase, (load, ms) => load.WriteGCodeFile(ms));
            await CallWriteToDb("render.cb",   loadBase, (load, ms) => load.WriteCamBamFile(ms));
            await CallWriteToDb("render.hpgl", loadBase, (load, ms) => load.WriteImportInfoFile(ms));
        }

        return loadBase;
    }

    private async Task CallWriteToDb(string dbFileName, LoadBase load, Action<LoadBase, StreamWriter> write)
    {
        using (var memoryStream = new MemoryStream())
        using (var sw = new StreamWriter(memoryStream))
        {
            write(load, sw);
            await sw.FlushAsync();

            var userFileDto = new UserFile()
            {
                FileName = dbFileName,
                UserId   = _userContext.UserId,
                Content  = memoryStream.ToArray()
            };

            var fileIdFromUri = await _fileManager.GetFileIdAsync(dbFileName);
            if (fileIdFromUri > 0)
            {
                userFileDto.UserFileId = fileIdFromUri;
                var fileIdFromValue = await _fileManager.GetFileIdAsync(dbFileName);
                await _fileManager.UpdateAsync(userFileDto);
            }
            else
            {
                await _fileManager.AddAsync(userFileDto);
            }
        }
    }

    private LoadBase CallLoadWithFileName(LoadOptions opt)
    {
        var load = LoadBase.Create(opt);

        if (load == null)
        {
            return null;
        }

        load.Load();
        return load;
    }

    private LoadBase CallLoadWithContent(LoadOptions opt)
    {
        var filename    = opt.FileName;
        var fileContent = opt.FileContent;

        string pathFileName = Path.GetFileName(filename);
        string tmpFile      = Path.GetTempPath() + pathFileName;

        opt.FileName             = tmpFile;
        opt.ImageWriteToFileName = null;

        try
        {
            File.WriteAllBytes(tmpFile, fileContent);

            LoadBase load = LoadBase.Create(opt);

            if (load == null)
            {
                return null;
            }

            load.Load();
            return load;
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }
}