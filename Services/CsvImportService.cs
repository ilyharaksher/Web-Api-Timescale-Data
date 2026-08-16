using Microsoft.AspNetCore.Http.HttpResults;
using System.Globalization;
using Web_Api_Timescale_Data.Entities;
using Web_Api_Timescale_Data.Exceptions;

namespace Web_Api_Timescale_Data.Services
{
    public class CsvImportService : ICsvImportService
    {
        public async Task ImportAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            string fileName = file.FileName;

            var header = await reader.ReadLineAsync();

            if (header != "Date;ExecutionTime;Value")
            {
                throw new CsvValidationException("Неверный формат заголовков. Необходимый формат: \"Date;ExecutionTime;Value\"");
            }

            var line = await reader.ReadLineAsync();
            int lineNumber = 1;

            // проверка структуры строки
            // пробразование типов
            // проверка значений согласно файлу
            // добавить ValueEntity в коллекцию
            // проверка кол-ва ValueEntity (добавить проверку длины списка в while)
            // посчитать ResultEntity
            // .db.SaveChanges();

            // spisok значений, который будем кидать в табличку values
            var ValuesList = new List<ValueEntity>();

            while (line != null) // && ValuesList.Count < 10000
            {
                if (ValuesList.Count > 10000)
                {
                    throw new CsvValidationException("В файле более 10000 строк");            
                }

                string[] parts = line.Split(";");

                if (parts.Length != 3)
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Запись должна содержать Date, ExecutionTime, Value");
                }

                // проверка типа для Date
                if (!DateTime.TryParseExact(
                    parts[0],
                    "yyyy-MM-dd'T'HH-mm-ss-ffff'Z'",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out DateTime date
                    ))
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Неверный формат даты");
                }

                // проверка типа для ExecutionTime
                if (!double.TryParse(parts[1], out double executionTime))
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Неверный формат ExecutionTime");
                }
                // проверка значения ExecutionTime
                if (executionTime < 0)
                {
                    return; // ошибка значения ExecutionTime
                }

                // проверка типа для Value
                if (!double.TryParse(parts[2], out double value))
                {
                    return; // ошибка типа для Value
                }

                if (value < 0)
                {
                    return; // ошибка значения Value
                }

                ValueEntity measure = new ValueEntity
                {
                    FileName = fileName,
                    Date = date,
                    ExecutionTime = executionTime,
                    Value = value
                };

                ValuesList.Add(measure);

                line = await reader.ReadLineAsync();
            }
        }
    }
}