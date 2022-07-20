using Coletor.Models;
using Coletor.Models.ViewModels;
using Coletor.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Coletor.Controllers
{
    public class CollectorsController : Controller
    {

        private readonly CollectorService _collectorService;

        public CollectorsController(CollectorService collectorService)
        {
            _collectorService = collectorService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var list = await _collectorService.FindAllAsync(page);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewFileType(SearchFilesViewModel vm)
        {
            try
            {
                await _collectorService.AddFileType(vm.NewFileTypeName);
                vm.NewFileTypeName = "";
                return RedirectToAction("SearchFiles", "Collectors", vm);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }

        }

        public async Task<IActionResult> SearchFiles()
        {
            var types = await _collectorService.FindAllFileTypesAsync();
            return View(new SearchFilesViewModel(types));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchFiles(SearchFilesViewModel vm)
        {
            try
            {
                int id = await _collectorService.RegisterDocumentAsync(vm);
                return RedirectToAction("Details", new {Id = id});
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }


        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }

            var obj = await _collectorService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            return View(obj);
        }


        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }

            var obj = await _collectorService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });

            }
            try
            {
                var fileBytes = System.IO.File.ReadAllBytes(obj.InternFilePath);
                return File(fileBytes, "Application/vnd.openxmlformats-officedocument.spreadsheetml.template", obj.InternFileName);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }

        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(viewModel);
        }


    }
}
