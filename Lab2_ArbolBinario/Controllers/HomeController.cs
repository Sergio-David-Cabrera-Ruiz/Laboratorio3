using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InventarioBST.Models;
using InventarioBST.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace InventarioBST.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISingleton _singleton;
        private static Random _random = new Random();

        public HomeController(ILogger<HomeController> logger, ISingleton singleton)
        {
            _logger = logger;
            _singleton = singleton;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Grid(DataTableRequest data)
        {
            DataTableResponse output = new DataTableResponse() { Draw = data.Draw };

            if (data.Search.ContainsKey("value") && !string.IsNullOrWhiteSpace(data.Search["value"]))
            {
                // Search by name in the tree
            }
            else
            {
                // Pagination directly on datasource
                output.Data = _singleton.DataSource.Where(d => d.Existencia > 0).Skip(data.Start).Take(data.Length).ToList();
                output.RecordsTotal = _singleton.DataSource.Where(d => d.Existencia > 0).Count();
                output.RecordsFiltered = output.RecordsTotal;
            }

            return Ok(output);
        }

        public IActionResult ConfirmOrder(Dictionary<int, int> data)
        {
            BasicResponse output = new BasicResponse();

            try
            {
                // Update counters on datasource
                if (data?.Count > 0)
                {
                    foreach (var item in data)
                    {
                        _singleton.DataSource[item.Key - 1].Existencia -= item.Value;
                    }
                }

                // Recalculate tree

                output.Success = true;
                output.Message = "Pedido confirmado";
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Message = "Ocurrio un error al confirmar el pedido";

                _logger.LogError(ex.Message);
            }

            return Ok(output);
        }

        public IActionResult ResetInventory()
        {
            BasicResponse output = new BasicResponse();
            try
            {
                for (int i = 0; i < _singleton.DataSource.Count; i++)
                {
                    _singleton.DataSource[i].Existencia = _random.Next(1, 15);
                }


                // Recalculate tree

                output.Success = true;
                output.Message = "Inventario reabastecido";
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Message = "Ocurrio un error al confirmar el pedido";

                _logger.LogError(ex.Message);
            }

            return Ok(output);
        }

        public IActionResult UploadFile(IFormFile file)
        {
            BasicResponse output = new BasicResponse();

            if (file.Length > 0)
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Get records
                    try
                    {
                        var records = csv.GetRecords<InventoryItem>();

                        // Clean singleton list
                        _singleton.DataSource.Clear();

                        foreach (var record in records)
                        {
                            // Save records to singleton
                            _singleton.DataSource.Add(record);
                        }

                        output.Success = true;
                        output.Message = $"Datos cargados: {_singleton.DataSource.Count}";
                    }
                    catch (Exception ex)
                    {
                        output.Success = false;
                        output.Message = "Ocurrio un error al agregar los elementos del archivo";

                        _logger.LogError(ex.Message);
                    }
                }
            }
            else
            {
                output.Success = false;
                output.Message = "Debe ingresar un archivo con información";
            }

            return Ok(output);
        }

        public IActionResult Download()
        {
            MemoryStream output = new MemoryStream();

            var writer = new StreamWriter(output);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(_singleton.DataSource);

            return File(output.ToArray(), "text/csv", "Inventario.csv");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
