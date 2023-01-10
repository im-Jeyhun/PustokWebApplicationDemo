using DemoApplication.Areas.Admin.ViewModels.Slider;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/slider")]
    [Authorize(Roles = "admin")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IFileService _fileService;
        public SliderController(DataContext dataContext, IFileService fileService)
        {
            _dataContext = dataContext;
            _fileService = fileService;
        }

        [HttpGet("list", Name = "admin-slider-list")]
        public async Task<IActionResult> ListAsync()
        {
            var model = await _dataContext.Sliders.Select(s => new ListItemViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Content = s.Content,
                BackgroundImageUrl = _fileService.GetFileUrl(s.BgImageNameInFileSystem, UploadDirectory.Slider)
            }).ToListAsync();

            return View(model);
        }
        [HttpGet("add-slider", Name = "admin-slider-add")]
        public async Task<IActionResult> AddAsync()
        {
            return View();
        }

        [HttpPost("add-slider", Name = "admin-slider-add")]
        public async Task<IActionResult> AddAsync(AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var BgImageNameInFileSytstem = await _fileService.UploadAsync(model.BgImage, UploadDirectory.Slider);

            AddSlider(model.BgImage.FileName, BgImageNameInFileSytstem);

            return RedirectToRoute("admin-slider-list");

            void AddSlider(string BgImageName, string BgImageNameInFileSystem)
            {
                var slider = new Slider
                {
                    Title = model.Title,
                    Content = model.Content,
                    BgImageName = BgImageName,
                    BgImageNameInFileSystem = BgImageNameInFileSystem,
                    ButtonName = model.ButtonName,
                    BtnRedirectUrl = model.BtnRedirectUrl,
                    Order = model.Order,

                };

                _dataContext.Add(slider);

                _dataContext.SaveChanges();
            }
        }

        [HttpGet("slider-update/{id}", Name = "admin-slider-update")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id)
        {

            var slider = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);

            if (slider is null)
            {
                return NotFound();
            }

            var model = new UpdateViewModel
            {
                Id = slider.Id,
                Title = slider.Title,
                Content = slider.Content,
                BackgroundImageUrl= _fileService.GetFileUrl(slider.BgImageNameInFileSystem , UploadDirectory.Slider),
                ButtonName = slider.ButtonName,
                BtnRedirectUrl = slider.BtnRedirectUrl,
                Order = slider.Order
            };
           
            return View(model);
        }

        [HttpPost("slider-update/{id}", Name = "admin-slider-update")]
        public async Task<IActionResult> UpdateAsync(UpdateViewModel model)
        {
            var slider = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Id == model.Id);

            if (!ModelState.IsValid)
            {
                GetView(model);
            }

            if (slider is null)
            {
                return NotFound();
            }

            if(model.BgImage is not null)
            {
                await _fileService.DeleteAsync(slider.BgImageNameInFileSystem, UploadDirectory.Slider);
                var imageFileNameInSystem = await _fileService.UploadAsync(model.BgImage, UploadDirectory.Slider);
                await UpdateSliderAsync(slider.BgImageName, imageFileNameInSystem);

            }
            else
            {
                UpdateSlider();
            }




            IActionResult GetView(UpdateViewModel model)
            {
                model.BackgroundImageUrl = _fileService.GetFileUrl(slider.BgImageNameInFileSystem, UploadDirectory.Slider);
                return View(model);
            }


            void UpdateSlider()
            {
                slider.Title = model.Title;
                slider.Content = model.Content;
                slider.ButtonName = model.ButtonName;
                slider.BtnRedirectUrl = model.BtnRedirectUrl;
                slider.Order = model.Order;

                _dataContext.SaveChanges();

            };

            async Task UpdateSliderAsync(string BgImageName, string BgImageNameInFileSystem)
            {
                slider.Title = model.Title;
                slider.Content = model.Content;
                slider.BgImageName = BgImageName;
                slider.BgImageNameInFileSystem = BgImageNameInFileSystem;
                slider.ButtonName = model.ButtonName;
                slider.BtnRedirectUrl = model.BtnRedirectUrl;
                slider.Order = model.Order;

               await _dataContext.SaveChangesAsync();

            }

            

            return RedirectToRoute("admin-slider-list");
        }

        [HttpPost("slider-delete/{id}" , Name ="admin-slider-delete" )]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var slider = await _dataContext.Sliders.FirstOrDefaultAsync(b => b.Id == id);
            if (slider is null)
            {
                return NotFound();
            }

            await _fileService.DeleteAsync(slider.BgImageNameInFileSystem, UploadDirectory.Slider);

            _dataContext.Sliders.Remove(slider);

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-slider-list");
        }

    }
}
