using BookNest.IServices;
using BookNest.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace BookNest.Controllers
{
    public class ClubHostController : Controller
    {
        private readonly IClubHostService _clubHostService;
        public ClubHostController(IClubHostService clubHostService)
        {
            _clubHostService = clubHostService;
        }
        public async Task<IActionResult> Index()
        {
            var rooms = await _clubHostService.GetAllRooms();
            return View(rooms);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ClubHostRoom room)
        {
           if (room == null) 
                return BadRequest("Room data is required.");
            var username = HttpContext.Session.GetString("UserName");
            await _clubHostService.CreateRoom(room, username);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult JoinRoom(int roomId, string studentName)
        {
            if(roomId <= 0 || studentName == null
                || string.IsNullOrEmpty(studentName))
            {
                return BadRequest(new { success = false, message = "Invalid room ID or student name" });
            }
            var member = _clubHostService.JoinMember(roomId, studentName);
            if (member == null)
                return Unauthorized(new { success = false ,message = "Unable to join room. Please check the room ID and try again." });
            return Ok(new { success = true, roomId = roomId, studentName = studentName , message="Added member Successfully"});
        }
        public async Task<IActionResult> UploadFile(List<IFormFile> files, int roomId)
        {
            var uploadedFiles = new List<object>();

            foreach (var file in files)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var dbPath = "/uploads/" + fileName;

                var attachment = new ClubHostAttachment
                {
                    RoomId = roomId,
                    FilePath = dbPath,
                    FileName = file.FileName,
                    UploadedBy = HttpContext.Session.GetString("UserName") ?? "Unknown",
                    UploadedAt = DateTime.UtcNow
                };

                await _clubHostService.AddAttachment(attachment);

                uploadedFiles.Add(new
                {
                    filePath = dbPath,
                    fileName = file.FileName
                });
            }

            return Json(uploadedFiles);
        }
        //[HttpPost]
        //public async Task<IActionResult> UploadFile(List<IFormFile> files, int roomId)
        //{
        //    List<string> filePaths = new List<string>();

        //    foreach (var file in files)
        //    {
        //        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        var dbPath = "/uploads/" + fileName;
        //        filePaths.Add(dbPath);

        //        var attachment = new ClubHostAttachment
        //        {
        //            RoomId = roomId,
        //            FilePath = dbPath,
        //            FileName = file.FileName,
        //            UploadedBy = HttpContext.Session.GetString("UserName") ?? "Unknown",
        //            UploadedAt = DateTime.UtcNow

        //        };
        //       await _clubHostService.AddAttachment(attachment);
        //    }
        //    return Json(new { filePaths });
        //}
    }
}
