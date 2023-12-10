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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Shared;

using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using UserFileDto = CNCLib.Logic.Abstraction.DTO.UserFile;

[Authorize]
[Route("api/[controller]")]
public class UserFileController : Controller
{
    private readonly IUserFileManager   _manager;
    private readonly ICNCLibUserContext _userContext;

    public UserFileController(IUserFileManager manager, ICNCLibUserContext userContext)
    {
        _manager     = manager;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserFileInfo>>> GetAll(string wildcard)
    {
        return Ok(await _manager.GetFileInfosAsync());
    }

    #region default REST

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("download")]
    public async Task<IActionResult> Get(string fileName)
    {
        var dto = await _manager.GetAsync(await GetKey(fileName));

        if (dto == null || dto.Content == null)
        {
            return NotFound();
        }

        var memoryStream = new MemoryStream(dto.Content);
        var name         = Path.GetFileName(dto.FileName);
        memoryStream.Position = 0;
        return File(memoryStream, this.GetContentType(name), name);
    }

    public class UserFile
    {
        public string? FileName { get; set; }

        public IFormFile? Image { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult<UserFile>> Add([FromForm] UserFile value)
    {
        var userFileDto = GetUserFileDto(value);

        if (userFileDto != null)
        {
            var userFile     = await this.Add(_manager, userFileDto);
            var userFileInfo = (await _manager.GetFileInfoAsync((UserFileDto)((CreatedResult)userFile.Result!).Value!))!;
            var newUri       = this.GetCurrentUri() + "/" + userFileDto.FileName;
            return Created(newUri, userFileInfo);
        }

        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> Update(string fileName, [FromForm] UserFile value)
    {
        var userFileDto = GetUserFileDto(value);

        if (userFileDto != null)
        {
            var fileIdFromUri = await _manager.GetFileIdAsync(fileName);
            if (fileIdFromUri > 0)
            {
                userFileDto.UserFileId = fileIdFromUri;
                var fileIdFromValue = await _manager.GetFileIdAsync(value.FileName!);
                await this.Update(_manager, fileIdFromUri, fileIdFromValue, userFileDto);
            }
            else
            {
                await this.Add(_manager, userFileDto);
            }

            return Ok();
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(string fileName)
    {
        var fileIdFromUri = await _manager.GetFileIdAsync(fileName);
        if (fileIdFromUri > 0)
        {
            return await this.Delete(_manager, fileIdFromUri);
        }

        return NotFound();
    }

    #endregion

    private UserFileDto GetUserFileDto(UserFile value)
    {
        using (var memoryStream = new MemoryStream())
        {
            value.Image!.CopyTo(memoryStream);
            return new UserFileDto() { UserId = _userContext.UserId, FileName = value.FileName!, Content = memoryStream.ToArray() };
        }
    }

    private async Task<int> GetKey(string filename)
    {
        return await _manager.GetFileIdAsync(filename);
    }
}