using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.Configuration.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;


namespace ASC.Web.Areas.Configuration.Controllers
{
    [Area("Configuration")]
    [Authorize(Roles = "Admin")]
    public class MasterDataController : Controller
    {
        private readonly IMasterDataOperations _masterDataOperations;
        private readonly IMapper _mapper;

        public MasterDataController(
            IMasterDataOperations masterDataOperations,
            IMapper mapper)
        {
            _masterDataOperations = masterDataOperations;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> MasterKeys()
        {
            var masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();

            var model = new MasterKeysViewModel
            {
                MasterKeys = _mapper.Map<List<MasterDataKeyViewModel>>(masterKeys),
                MasterKey = new MasterDataKeyViewModel()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterKeys(MasterKeysViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();
                model.MasterKeys = _mapper.Map<List<MasterDataKeyViewModel>>(masterKeys);

                return View(model);
            }

            var masterDataKey = _mapper.Map<MasterDataKey>(model.MasterKey);

            if (string.IsNullOrWhiteSpace(model.MasterKey.Id))
            {
                var result = await _masterDataOperations.InsertMasterKeyAsync(masterDataKey);

                if (result)
                {
                    TempData["SuccessMessage"] = "Master Key created successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Cannot create Master Key.";
                }
            }
            else
            {
                var result = await _masterDataOperations.UpdateMasterKeyAsync(masterDataKey);

                if (result)
                {
                    TempData["SuccessMessage"] = "Master Key updated successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Cannot update Master Key.";
                }
            }

            return RedirectToAction(nameof(MasterKeys));
        }

        [HttpGet]
        public async Task<IActionResult> MasterValues()
        {
            var masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();
            var masterValues = await _masterDataOperations.GetAllMasterValuesAsync();

            var model = new MasterValuesViewModel
            {
                MasterKeys = masterKeys
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Name)
                    .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Name
                    })
                    .ToList(),

                MasterValues = _mapper.Map<List<MasterDataValueViewModel>>(masterValues)
            };

            model.MasterValue.MasterKeys = model.MasterKeys;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterValues(MasterValuesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadMasterValuesModelAsync(model);
                return View(model);
            }

            var masterDataValue = _mapper.Map<MasterDataValue>(model.MasterValue);

            if (string.IsNullOrWhiteSpace(model.MasterValue.Id))
            {
                var result = await _masterDataOperations.InsertMasterValueAsync(masterDataValue);

                if (result)
                {
                    TempData["SuccessMessage"] = "Master Value created successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Cannot create Master Value.";
                }
            }
            else
            {
                var result = await _masterDataOperations.UpdateMasterValueAsync(masterDataValue);

                if (result)
                {
                    TempData["SuccessMessage"] = "Master Value updated successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Cannot update Master Value.";
                }
            }

            return RedirectToAction(nameof(MasterValues));
        }

        private async Task ReloadMasterValuesModelAsync(MasterValuesViewModel model)
        {
            var masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();
            var masterValues = await _masterDataOperations.GetAllMasterValuesAsync();

            model.MasterKeys = masterKeys
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = x.Id,
                    Text = x.Name
                })
                .ToList();

            model.MasterValues = _mapper.Map<List<MasterDataValueViewModel>>(masterValues);
            model.MasterValue.MasterKeys = model.MasterKeys;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportMasterValues(IFormFile importFile)
        {
            if (importFile == null || importFile.Length == 0)
            {
                TempData["SuccessMessage"] = "Please select an Excel file.";
                return RedirectToAction(nameof(MasterValues));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();
            var valuesToImport = new List<MasterDataValue>();

            using (var stream = new MemoryStream())
            {
                await importFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet == null)
                    {
                        TempData["SuccessMessage"] = "Excel file does not contain any worksheet.";
                        return RedirectToAction(nameof(MasterValues));
                    }

                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var masterKeyName = worksheet.Cells[row, 1].Text?.Trim();
                        var masterValueName = worksheet.Cells[row, 2].Text?.Trim();
                        var isActiveText = worksheet.Cells[row, 3].Text?.Trim();

                        if (string.IsNullOrWhiteSpace(masterKeyName) ||
                            string.IsNullOrWhiteSpace(masterValueName))
                        {
                            continue;
                        }

                        var masterKey = masterKeys.FirstOrDefault(x =>
                            x.Name.Equals(masterKeyName, StringComparison.OrdinalIgnoreCase));

                        if (masterKey == null)
                        {
                            masterKey = new MasterDataKey
                            {
                                Name = masterKeyName,
                                IsActive = true
                            };

                            await _masterDataOperations.InsertMasterKeyAsync(masterKey);

                            masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();

                            masterKey = masterKeys.FirstOrDefault(x =>
                                x.Name.Equals(masterKeyName, StringComparison.OrdinalIgnoreCase));

                            if (masterKey == null)
                            {
                                continue;
                            }
                        }

                        var isActive = true;

                        if (!string.IsNullOrWhiteSpace(isActiveText))
                        {
                            bool.TryParse(isActiveText, out isActive);
                        }

                        valuesToImport.Add(new MasterDataValue
                        {
                            MasterDataKeyId = masterKey.Id,
                            Value = masterValueName,
                            IsActive = isActive
                        });
                    }
                }
            }

            if (!valuesToImport.Any())
            {
                TempData["SuccessMessage"] = "No valid data found in Excel file.";
                return RedirectToAction(nameof(MasterValues));
            }

            var result = await _masterDataOperations.UploadMasterDataAsync(valuesToImport);

            TempData["SuccessMessage"] = result
                ? $"Imported {valuesToImport.Count} master values successfully."
                : "Import failed.";

            return RedirectToAction(nameof(MasterValues));
        }
    }
}