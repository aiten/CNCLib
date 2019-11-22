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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Shared;

using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using UserFileDto = CNCLib.Logic.Abstraction.DTO.UserFile;

namespace CNCLib.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserFileController : Controller
    {
        private readonly IUserFileManager   _manager;
        private readonly ICNCLibUserContext _userContext;

        public UserFileController(IUserFileManager manager, ICNCLibUserContext userContext)
        {
            _manager     = manager ?? throw new ArgumentNullException(nameof(manager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAll(string wildcard)
        {
            return Ok(await _manager.GetFileNames());
        }

        #region default REST

        [HttpGet("{filename}")]
        public async Task<IActionResult> Get(string filename)
        {
            var dto = await _manager.Get(GetKey(filename));

            if (dto == null)
            {
                return NotFound();
            }

            var memoryStream = new MemoryStream(dto.Content);
            var fileName = Path.GetFileName(dto.FileName);
            return await this.GetFile(fileName, memoryStream);
        }
        
        public class UserFile
        {
            public string FileName { get; set; }

            public IFormFile Image { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<string>> Add([FromForm] UserFile value)
        {
            var userFileDto = GetUserFileDto(value);

            if (userFileDto != null)
            {
                await this.Add<UserFileDto, Tuple<int, string>>(_manager, userFileDto);
                return Ok(value.FileName);
            }

            return NoContent();
        }

        [HttpPut("{filename}")]
        public async Task<ActionResult> Update(string filename, [FromForm] UserFile value)
        {
            var userFileDto = GetUserFileDto(value);

            if (userFileDto != null)
            {
                await this.Update<UserFileDto, Tuple<int, string>>(_manager, GetKey(filename), GetKey(value.FileName), userFileDto);
                return Ok(value.FileName);
            }

            return NoContent();
        }

        [HttpDelete("{filename}")]
        public async Task<ActionResult> Delete(string filename)
        {
            return await this.Delete<UserFileDto, Tuple<int, string>>(_manager, GetKey(filename));
        }

        #endregion
        private UserFileDto GetUserFileDto(UserFile value)
        {
            using (var memoryStream = new MemoryStream())
            {
                value.Image.CopyTo(memoryStream);
                return new UserFileDto() { UserId = _userContext.UserId ?? 0, FileName = value.FileName, Content = memoryStream.ToArray() };
            }
        }


        private Tuple<int, string> GetKey(string filename)
        {
            return new Tuple<int, string>(_userContext.UserId??0, filename);
        }
    }
}