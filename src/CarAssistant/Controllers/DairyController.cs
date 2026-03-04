using System.Security.Claims;
using CarAssistant.Features.Dairy.Commands;
using CarAssistant.Features.Dairy.Models;
using CarAssistant.Features.Dairy.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CarAssistant.Controllers;

[Authorize]
public class DairyController : Controller
{
    private readonly GetDairyEntriesQueryHandler _getHandler;
    private readonly SaveDairyEntryCommandHandler _saveHandler;
    private readonly DeleteDairyEntryCommandHandler _deleteHandler;
    private readonly IWebHostEnvironment _env;

    public DairyController(GetDairyEntriesQueryHandler getHandler, SaveDairyEntryCommandHandler saveHandler, DeleteDairyEntryCommandHandler deleteHandler, IWebHostEnvironment env)
    {
        _getHandler = getHandler;
        _saveHandler = saveHandler;
        _deleteHandler = deleteHandler;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId == 0)
        {
            return RedirectToAction("Index", "Auth");
        }

        var entries = await _getHandler.Handle(new GetDairyEntriesQuery(userId));
        return View(entries);
    }

    [HttpPost]
    public async Task<IActionResult> Save(DairyEntryInputModel model, List<IFormFile>? photos, List<string>? existingPhotoUrls)
    {
        var userId = GetUserId();
        if (userId == 0)
        {
            return RedirectToAction("Index", "Auth");
        }

        if (!ModelState.IsValid)
        {
            var entries = await _getHandler.Handle(new GetDairyEntriesQuery(userId));
            return View("Index", entries);
        }

        var photoUrls = new List<string>();

        if (photos is { Count: > 0 })
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "dairy");
            Directory.CreateDirectory(uploadsRoot);

            foreach (var file in photos.Where(f => f != null && f.Length > 0).Take(10))
            {
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = $"/uploads/dairy/{fileName}";
                photoUrls.Add(relativeUrl);
            }
        }

        // Объединяем уже существующие фото, которые пользователь оставил, и новые загруженные
        if (existingPhotoUrls is { Count: > 0 })
        {
            photoUrls.InsertRange(0, existingPhotoUrls);
        }

        await _saveHandler.Handle(new SaveDairyEntryCommand(model.Id, model.Title, model.Text, userId, photoUrls));

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == 0)
        {
            return RedirectToAction("Index", "Auth");
        }

        await _deleteHandler.Handle(new DeleteDairyEntryCommand(id, userId));

        return RedirectToAction("Index");
    }

    private int GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}