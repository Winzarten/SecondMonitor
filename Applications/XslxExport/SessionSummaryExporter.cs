namespace SecondMonitor.XslxExport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;

    using DataModel.BasicProperties;
    using DataModel.Summary;

    using NLog;

    using OfficeOpenXml;
    using OfficeOpenXml.Drawing.Chart;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;

    using DataModel.Extensions;
    using DataModel.Snapshot.Systems;
    using WindowsControls.Colors;

    public class SessionSummaryExporter
    {
        private const string SummarySheet = "Summary";
        private const string LapsAndSectorsSheet = "Laps & Sectors";
        private const string PlayerLapsSheet = "Players Laps";
        private const string RaceProgressSheet = "RaceProgress";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Color PersonalBestColor { get; set; } = Colors.Green;

        public Color SessionBestColor { get; set; } = Colors.Purple;

        public Color InvalidColor { get; set; } = Color.FromRgb(189, 7, 59);

        public VelocityUnits VelocityUnits { get; set; } = VelocityUnits.Kph;

        public TemperatureUnits TemperatureUnits { get; set; } = TemperatureUnits.Celsius;

        public VolumeUnits VolumeUnits { get; set; } = VolumeUnits.Liters;

        public PressureUnits PressureUnits { get; set; } = PressureUnits.Kpa;

        public void ExportSessionSummary(SessionSummary sessionSummary, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileInfo newFile = new FileInfo(filePath);
                ExcelPackage package = new ExcelPackage(newFile);

                CreateWorkBook(package, sessionSummary.SessionType == SessionType.Race);
                ExcelWorkbook workbook = package.Workbook;
                AddSummary(workbook.Worksheets[SummarySheet], sessionSummary);
                AddLapsInfo(workbook.Worksheets[LapsAndSectorsSheet], sessionSummary);
                AddPlayersLaps(workbook.Worksheets[PlayerLapsSheet], sessionSummary);

                if (sessionSummary.SessionType == SessionType.Race)
                {
                    AddRaceProgress(workbook.Worksheets[RaceProgressSheet], sessionSummary);
                }

                package.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to export session info");
            }

        }

        private void AddRaceProgress(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            int maxLaps = sessionSummary.Drivers.Select(x => x.Laps.Count).Max();
            List<Driver> orderedDrivers = sessionSummary.Drivers.OrderBy(x => x.Laps.LastOrDefault()?.LapEndSnapshot.PlayerData.Position).ToList();
            int startRow = 100;
            int startColumn = 1;
            sheet.Cells[startRow + 1, startColumn].Value = "Start";
            GenerateNumberColumn(sheet, new ExcelCellAddress(startRow + 2, startColumn), maxLaps);
            GenerateDriversRow(sheet, new ExcelCellAddress(startRow, startColumn + 1), orderedDrivers.Select(x => x.DriverName));

            ExcelCellAddress startAddress = new ExcelCellAddress(startRow + 1, startColumn + 1);
            orderedDrivers.ForEach(
                x =>
                    {
                        GenerateLapsPositionColumn(sheet, startAddress, x.Laps, maxLaps);
                        startAddress = new ExcelCellAddress(startAddress.Row, startAddress.Column + 1);
                    });
            ExcelLineChart chart = (ExcelLineChart)sheet.Drawings.AddChart("Race Progress", eChartType.LineMarkers);
            chart.SetPosition(0, 0, 0, 0);
            int currentColumn = 2;
            orderedDrivers.ForEach(
                x =>
                    {
                        ExcelLineChartSerie series = (ExcelLineChartSerie) chart.Series.Add(ExcelCellBase.GetAddress(startRow + 1, currentColumn, startRow + 1 + maxLaps, currentColumn), ExcelCellBase.GetAddress(startRow + 1, 1, startRow + 1 + maxLaps, 1));
                        series.Header = x.DriverName;
                        currentColumn++;
                    });
            chart.ShowDataLabelsOverMaximum = false;
            chart.DataLabel.ShowValue = true;
            chart.ShowHiddenData = true;

            chart.Axis[1].MinValue = 0;
            chart.Axis[1].TickLabelPosition = eTickLabelPosition.NextTo;
            chart.Axis[1].MajorUnit = 1;
            chart.Axis[1].MinorUnit = 1;
            chart.Axis[1].Orientation = eAxisOrientation.MaxMin;
            chart.Axis[1].MaxValue = orderedDrivers.Count;
            chart.SetSize(70 * maxLaps, 30 * orderedDrivers.Count);
            chart.Axis[0].MajorUnit = 1;
            chart.Axis[0].MinorUnit = 1;
            chart.Title.Text = "Race Progress";
        }

        private void GenerateLapsPositionColumn(ExcelWorksheet sheet, ExcelCellAddress startAddress, IEnumerable<Lap> laps, int maxLaps)
        {
            List<Lap> lapsList = laps.ToList();
            if (!lapsList.Any())
            {
                return;
            }

            sheet.Cells[startAddress.Address].Value = lapsList.First().LapStartSnapshot.PlayerData.Position;
            startAddress = new ExcelCellAddress(startAddress.Row + 1, startAddress.Column);

            lapsList.ForEach(x =>
            {
                    sheet.Cells[startAddress.Address].Value = x.LapEndSnapshot?.PlayerData?.Position > 0 ? x.LapEndSnapshot.PlayerData.Position : x.Driver.FinishingPosition;
                    startAddress = new ExcelCellAddress(startAddress.Row + 1, startAddress.Column);
            });

            for (int i = lapsList.Count(); i < maxLaps; i++)
            {
                sheet.Cells[startAddress.Address].Value = lapsList.Last().Driver.FinishingPosition;
                startAddress = new ExcelCellAddress(startAddress.Row + 1, startAddress.Column);
            }
        }

        private void GenerateNumberColumn(ExcelWorksheet sheet, ExcelCellAddress cellAddress, int count)
        {
            for (int i = 0; i < count; i++)
            {
                sheet.Cells[cellAddress.Row + i, cellAddress.Column].Value = i + 1;
            }
        }

        private void GenerateDriversRow(ExcelWorksheet sheet, ExcelCellAddress cellAddress, IEnumerable<string> driverNames)
        {
            driverNames.ForEach(x =>
                {
                    sheet.Cells[cellAddress.Address].Value = x;
                    sheet.Cells[cellAddress.Address].AutoFitColumns();
                    cellAddress = new ExcelCellAddress(cellAddress.Row, cellAddress.Column + 1);
            });
        }

        private void AddPlayersLaps(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            AddPlayerLapsHeader(sheet);
            Driver player = sessionSummary.Drivers.FirstOrDefault(x => x.IsPlayer);
            if (player == null)
            {
                return;
            }

            Lap lastLap = null;
            int currentRow = 6;
            player.Laps.Where(l => l.LapEndSnapshot != null).ToList().ForEach(
                x =>
                    {
                        currentRow = AddPlayerLapInfo(sheet, sessionSummary, x, lastLap, currentRow);
                        lastLap = x;
                    });
            sheet.Cells["A1:M5"].AutoFitColumns();
        }

        private void AddLapsInfo(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            AddLapsInfoHeader(sheet, sessionSummary);
            int currentColumn = 2;
            foreach (Driver driver in sessionSummary.Drivers.Where(d => d.Finished).OrderBy(o => o.FinishingPosition))
            {
                AddDriverLaps(sheet, currentColumn, driver, sessionSummary);
                currentColumn = currentColumn + 4;
            }

            foreach (Driver driver in sessionSummary.Drivers.Where(d => !d.Finished).OrderBy(d => d.TotalLaps).Reverse())
            {
                AddDriverLaps(sheet, currentColumn, driver, sessionSummary);
                currentColumn = currentColumn + 4;
            }
        }

        private void AddDriverLaps(ExcelWorksheet sheet, int startColumn, Driver driver, SessionSummary sessionSummary)
        {
            ExcelRange range = sheet.Cells[1, startColumn, 1, startColumn + 3];
            range.Merge = true;
            range.Value = driver.DriverName + "(" + (driver.Finished ? driver.FinishingPosition.ToString(): "DNF") + ")";
            range.Style.Font.Bold = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[2, startColumn].Value = "Sector 1";
            sheet.Cells[2, startColumn + 1].Value = "Sector 2";
            sheet.Cells[2, startColumn + 2].Value = "Sector 3";
            sheet.Cells[2, startColumn + 3].Value = "Lap";

            int currentRow = 3;
            foreach (var lap in driver.Laps.OrderBy(l => l.LapNumber))
            {
                sheet.Cells[currentRow, startColumn].Value = FormatTimeSpan(lap.Sector1);
                if (lap == driver.BestSector1Lap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn]);
                }

                if (lap == sessionSummary.SessionBestSector1)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn]);
                }

                sheet.Cells[currentRow, startColumn + 1].Value = FormatTimeSpan(lap.Sector2);
                if (lap == driver.BestSector2Lap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn + 1]);
                }

                if (lap == sessionSummary.SessionBestSector2)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn + 1]);
                }

                sheet.Cells[currentRow, startColumn + 2].Value = FormatTimeSpan(lap.Sector3);
                if (lap == driver.BestSector3Lap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn + 2]);
                }

                if (lap == sessionSummary.SessionBestSector3)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn + 2]);
                }

                sheet.Cells[currentRow, startColumn + 3].Value = FormatTimeSpan(lap.LapTime);
                if (lap == driver.BestPersonalLap)
                {
                    FormatAsPersonalBest(sheet.Cells[currentRow, startColumn + 3]);
                }

                if (lap == sessionSummary.SessionBestLap)
                {
                    FormatAsSessionBest(sheet.Cells[currentRow, startColumn + 3]);
                }

                if (!lap.IsValid)
                {
                    FillWithColor(
                        sheet.Cells[currentRow, startColumn, currentRow, startColumn + 3],
                        Colors.White,
                        InvalidColor);
                }

                currentRow++;
            }

            ExcelRange outLineRange = sheet.Cells[1, startColumn, currentRow - 1, startColumn + 3];
            outLineRange.Style.Border.BorderAround(ExcelBorderStyle.Thick, System.Drawing.Color.Black);
        }

        private void FormatAsPersonalBest(ExcelRange range)
        {
            FillWithColor(range, Colors.White, PersonalBestColor);
        }

        private void FormatAsSessionBest(ExcelRange range)
        {
            FillWithColor(range, Colors.White,  SessionBestColor);
        }

        private static void FillWithColor(ExcelRange range, Color foregroundColor, Color bgColor)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Font.Color.SetColor(foregroundColor.ToDrawingColor());
            range.Style.Fill.BackgroundColor.SetColor(bgColor.ToDrawingColor());
            range.Style.Fill.PatternColor.SetColor(bgColor.ToDrawingColor());
        }

        private void AddLapsInfoHeader(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            int maxLaps = sessionSummary.Drivers.Select(d => d.TotalLaps).Max();
            sheet.SelectedRange["A1"].Value = "Lap/Driver";
            for (int i = 1; i <= maxLaps; i++)
            {
                sheet.SelectedRange["A" + (i + 2)].Value = i;
            }
            ExcelRange range = sheet.SelectedRange["A1:A" + 2 + maxLaps];
            range.Style.Font.Bold = true;
            sheet.Column(1).AutoFit();
            sheet.View.FreezePanes(1,2);
        }

        private void AddSummary(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            AddTrackInformation(sheet, sessionSummary);
            AddSessionBasicInformation(sheet, sessionSummary);
            AddDriversInfoHeader(sheet);
            AddDriversInfo(sheet, sessionSummary);
            WrapSummaryInTable(sheet, sessionSummary.Drivers.Count);
            AddSessionBestInfo(sessionSummary, sheet);
        }

        private static void WrapSummaryInTable(ExcelWorksheet sheet, int rowCount)
        {
            ExcelRange range = sheet.Cells[5, 1, 5 + rowCount, 9];
            ExcelTable table = sheet.Tables.Add(range, "SummaryTable");
            table.ShowHeader = true;
            for (int i = 1; i <= 9; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }

        private void AddSessionBasicInformation(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            sheet.Cells["A2"].Value = "Date: " + sessionSummary.SessionRunTime.Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            sheet.Cells["A3"].Value = "Time: " + sessionSummary.SessionRunTime.TimeOfDay.ToString(@"hh\:mm");
            sheet.Cells["A4"].Value = "Simulator: " + sessionSummary.Simulator;

            sheet.Cells[2,1,4,1].AutoFitColumns();
        }

        private void AddDriversInfo(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            int rowNum = 5;
            foreach (Driver driver in sessionSummary.Drivers.Where(d => d.Finished).OrderBy(driver => driver.FinishingPosition))
            {
                ExcelRow row = sheet.Row(rowNum);
                AddDriverInfo(sheet, row, driver, sessionSummary);
                rowNum++;
            }

            foreach (Driver driver in sessionSummary.Drivers.Where(d => !d.Finished).OrderBy(d => d.TotalLaps).Reverse())
            {
                ExcelRow row = sheet.Row(rowNum);
                AddDriverInfo(sheet, row, driver, sessionSummary);
                rowNum++;
            }

            for (int i = 1; i <= 9; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }

        private int AddPlayerLapInfo(ExcelWorksheet sheet, SessionSummary session, Lap lapInfo, Lap previousLap, int startRow)
        {
            CarInfo playerCarInfo = lapInfo.LapEndSnapshot.PlayerData.CarInfo;

            sheet.Cells[1 + startRow, 1].Value = lapInfo.IsValid ? lapInfo.LapNumber.ToString() : lapInfo.LapNumber + "(i)";
            sheet.Cells[1 + startRow, 2].Value = FormatTimeSpan(lapInfo.LapTime);

            if (lapInfo == lapInfo.Driver.BestPersonalLap)
            {
                FormatAsPersonalBest(sheet.Cells[1 + startRow, 2]);
            }
            if (lapInfo == session.SessionBestLap)
            {
                FormatAsSessionBest(sheet.Cells[1 + startRow, 2]);
            }


            sheet.Cells[1 + startRow, 5].Value = playerCarInfo.FuelSystemInfo.FuelRemaining.GetValueInUnits(VolumeUnits, 1);
            sheet.Cells[1 + startRow, 7].Value = GetBrakeTemperature(playerCarInfo.WheelsInfo.FrontLeft);
            sheet.Cells[1 + startRow, 8].Value = playerCarInfo.WheelsInfo.FrontLeft.BrakeTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits, 0);
            sheet.Cells[1 + startRow, 10].Value = GetBrakeTemperature(playerCarInfo.WheelsInfo.FrontRight);
            sheet.Cells[1 + startRow, 11].Value = playerCarInfo.WheelsInfo.FrontRight.BrakeTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits, 0);

            sheet.Cells[2 + startRow, 2].Value = FormatTimeSpan(lapInfo.Sector1);
            if (lapInfo == lapInfo.Driver.BestSector1Lap)
            {
                FormatAsPersonalBest(sheet.Cells[2 + startRow, 2]);
            }

            if (lapInfo == session.SessionBestSector1)
            {
                FormatAsSessionBest(sheet.Cells[2 + startRow, 2]);
            }

            sheet.Cells[2 + startRow, 3].Value = FormatTimeSpan(lapInfo.Sector2);
            if (lapInfo == lapInfo.Driver.BestSector2Lap)
            {
                FormatAsPersonalBest(sheet.Cells[2 + startRow, 3]);
            }

            if (lapInfo == session.SessionBestSector2)
            {
                FormatAsSessionBest(sheet.Cells[2 + startRow, 3]);
            }

            sheet.Cells[2 + startRow, 4].Value = FormatTimeSpan(lapInfo.Sector3);
            if (lapInfo == lapInfo.Driver.BestSector3Lap)
            {
                FormatAsPersonalBest(sheet.Cells[2 + startRow,4]);
            }

            if (lapInfo == session.SessionBestSector3)
            {
                FormatAsSessionBest(sheet.Cells[2 + startRow, 4]);
            }
            if (previousLap != null)
            {
                sheet.Cells[2 + startRow, 5].Value =
                    (playerCarInfo.FuelSystemInfo.FuelRemaining
                     - previousLap.LapEndSnapshot.PlayerData.CarInfo.FuelSystemInfo.FuelRemaining)
                    .GetValueInUnits(VolumeUnits, 2);
            }

            sheet.Cells[2 + startRow, 7].Value =
                playerCarInfo.WheelsInfo.FrontLeft.TyrePressure.ActualQuantity.GetValueInUnits(PressureUnits, 2);
            sheet.Cells[2 + startRow, 8].Value = ((1 - playerCarInfo.WheelsInfo.FrontLeft.TyreWear.ActualWear) * 100).ToString("F0");
            sheet.Cells[2 + startRow, 10].Value =
                playerCarInfo.WheelsInfo.FrontRight.TyrePressure.ActualQuantity.GetValueInUnits(PressureUnits, 2);
            sheet.Cells[2 + startRow, 11].Value = ((1 - playerCarInfo.WheelsInfo.FrontRight.TyreWear.ActualWear) * 100).ToString("F0");

            sheet.Cells[4 + startRow, 2].Value =
                lapInfo.LapEndSnapshot.WeatherInfo.AirTemperature.GetValueInUnits(TemperatureUnits, 1);
            sheet.Cells[4 + startRow, 3].Value = lapInfo.LapEndSnapshot.WeatherInfo.TrackTemperature.GetValueInUnits(TemperatureUnits, 1);
            sheet.Cells[4 + startRow, 4].Value = lapInfo.LapEndSnapshot.WeatherInfo.RainIntensity;
            sheet.Cells[4 + startRow, 5].Value =
                playerCarInfo.WaterSystemInfo.OptimalWaterTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits, 1);
            sheet.Cells[4 + startRow, 7].Value = GetBrakeTemperature(playerCarInfo.WheelsInfo.RearLeft);
            sheet.Cells[4 + startRow, 8].Value = playerCarInfo.WheelsInfo.RearLeft.BrakeTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits, 0);
            sheet.Cells[4 + startRow, 10].Value = GetBrakeTemperature(playerCarInfo.WheelsInfo.RearRight);
            sheet.Cells[4 + startRow, 11].Value = playerCarInfo.WheelsInfo.RearRight.BrakeTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits, 0);

            sheet.Cells[5 + startRow, 5].Value =
                playerCarInfo.OilSystemInfo.OptimalOilTemperature.ActualQuantity.GetValueInUnits(TemperatureUnits, 1);
            sheet.Cells[5 + startRow, 7].Value =
                playerCarInfo.WheelsInfo.RearLeft.TyrePressure.ActualQuantity.GetValueInUnits(PressureUnits, 2);
            sheet.Cells[5 + startRow, 8].Value = ((1 - playerCarInfo.WheelsInfo.RearLeft.TyreWear.ActualWear) * 100).ToString("F0");
            sheet.Cells[5 + startRow, 10].Value =
                playerCarInfo.WheelsInfo.RearRight.TyrePressure.ActualQuantity.GetValueInUnits(PressureUnits, 2);
            sheet.Cells[5 + startRow, 11].Value = ((1 - playerCarInfo.WheelsInfo.RearRight.TyreWear.ActualWear) * 100).ToString("F0");

            ExcelRange range = sheet.Cells[startRow + 1,1, startRow + 5,1];
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Merge = true;

            range = sheet.Cells[startRow + 1, 2, startRow + 1, 4];
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Merge = true;

            range = sheet.Cells[startRow + 1, 7, startRow + 2, 8];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells[startRow + 1, 10, startRow + 2, 11];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells[startRow + 4, 7, startRow + 5, 8];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells[startRow + 4, 10, startRow + 5, 11];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells[startRow + 1, 1, startRow + 5, 11];
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Thick, System.Drawing.Color.Black);

            return startRow + 6;
        }

        private string GetBrakeTemperature(WheelInfo wheel)
        {
            return wheel.LeftTyreTemp.ActualQuantity.GetValueInUnits(TemperatureUnits, 0) + @"/"
                                                                           + wheel.CenterTyreTemp.ActualQuantity.GetValueInUnits(
                                                                               TemperatureUnits,
                                                                               0) + @"/" + wheel.RightTyreTemp.ActualQuantity
                                                                               .GetValueInUnits(TemperatureUnits, 0);
        }

        private void AddPlayerLapsHeader(ExcelWorksheet sheet)
        {
            sheet.Cells["A1"].Value = "Lap #";
            sheet.Cells["B1"].Value = "Lap Time";
            sheet.Cells["E1"].Value = "Fuel";
            sheet.Cells["G1"].Value = "LF Temp";
            sheet.Cells["H1"].Value = "Brake Temp";
            sheet.Cells["J1"].Value = "FR Temp";
            sheet.Cells["K1"].Value = "BrakeTemp";

            sheet.Cells["B2"].Value = "Sector 1";
            sheet.Cells["C2"].Value = "Sector 2";
            sheet.Cells["D2"].Value = "Sector 3";
            sheet.Cells["E2"].Value = "Fuel Change";
            sheet.Cells["G2"].Value = "LF Pressure";
            sheet.Cells["H2"].Value = "Brake Temp";
            sheet.Cells["J2"].Value = "FR Pressure";
            sheet.Cells["K2"].Value = "Tyre Condition";

            sheet.Cells["B4"].Value = "Air Temp";
            sheet.Cells["C4"].Value = "Track Temp";
            sheet.Cells["D4"].Value = "Rain %";
            sheet.Cells["E4"].Value = "Water Temp";
            sheet.Cells["G4"].Value = "LR Temp";
            sheet.Cells["H4"].Value = "Brake Temp";
            sheet.Cells["J4"].Value = "RR Temp";
            sheet.Cells["K4"].Value = "BrakeTemp";

            sheet.Cells["E5"].Value = "Oil temp";
            sheet.Cells["G5"].Value = "LR Pressure";
            sheet.Cells["H5"].Value = "Tyre Condition";
            sheet.Cells["J5"].Value = "RR Pressure";
            sheet.Cells["K5"].Value = "Tyre Condition";

            sheet.Cells["M1"].Value = "UOM";
            sheet.Cells["M2"].Value = "Temperature: ";
            sheet.Cells["M3"].Value = "Pressure: ";
            sheet.Cells["M4"].Value = "Speed: ";
            sheet.Cells["M5"].Value = "Volume: ";
            sheet.Cells["N2"].Value = Temperature.GetUnitSymbol(TemperatureUnits);
            sheet.Cells["N3"].Value = Pressure.GetUnitSymbol(PressureUnits);
            sheet.Cells["N4"].Value = Velocity.GetUnitSymbol(VelocityUnits);
            sheet.Cells["N5"].Value = Volume.GetUnitSymbol(VolumeUnits);

            ExcelRange range = sheet.Cells["A1:A5"];
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Merge = true;

            range = sheet.Cells["B1:D1"];
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Merge = true;

            range = sheet.Cells["G1:H2"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells["J1:K2"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells["G4:H5"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells["J4:K5"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            range = sheet.Cells["M1:N1"];
            range.Style.Font.Bold = true;
            range.Merge = true;

            range = sheet.Cells["A1:K5"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Thick, System.Drawing.Color.Black);
            sheet.View.FreezePanes(7, 14);
        }

        private void AddDriverInfo(ExcelWorksheet sheet, ExcelRow row, Driver driver, SessionSummary sessionSummary)
        {
            sheet.Cells[row.Row + 1, 1].Value = GetFinishPositionInfo(driver, sessionSummary.SessionType == SessionType.Race);
            sheet.Cells[row.Row + 1, 2].Value = driver.DriverName;
            sheet.Cells[row.Row + 1, 3].Value = driver.CarName;
            sheet.Cells[row.Row + 1, 4].Value = driver.TotalLaps;
            sheet.Cells[row.Row + 1, 5].Value = driver.BestPersonalLap == null ? string.Empty : FormatTimeSpan(driver.BestPersonalLap.LapTime);
            sheet.Cells[row.Row + 1, 6].Value = driver.BestSector1Lap == null ? string.Empty : FormatTimeSpan(driver.BestSector1Lap.Sector1);
            sheet.Cells[row.Row + 1, 7].Value = driver.BestSector2Lap == null ? string.Empty : FormatTimeSpan(driver.BestSector2Lap.Sector2);
            sheet.Cells[row.Row + 1, 8].Value = driver.BestSector3Lap == null ? string.Empty : FormatTimeSpan(driver.BestSector3Lap.Sector3);
            sheet.Cells[row.Row + 1, 9].Value = driver.TopSpeed.GetValueInUnits(VelocityUnits).ToString("N0") + Velocity.GetUnitSymbol(VelocityUnits);

            if (driver.BestPersonalLap == sessionSummary.SessionBestLap)
            {
                sheet.Cells[row.Row + 1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 5].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
                sheet.Cells[row.Row + 1, 5].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            if (driver.BestSector1Lap == sessionSummary.SessionBestSector1)
            {
                sheet.Cells[row.Row + 1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 6].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
                sheet.Cells[row.Row + 1, 6].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            if (driver.BestSector2Lap == sessionSummary.SessionBestSector2)
            {
                sheet.Cells[row.Row + 1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 7].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
                sheet.Cells[row.Row + 1, 7].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            if (driver.BestSector3Lap == sessionSummary.SessionBestSector3)
            {
                sheet.Cells[row.Row + 1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row.Row + 1, 8].Style.Fill.BackgroundColor.SetColor(SessionBestColor.ToDrawingColor());
                sheet.Cells[row.Row + 1, 8].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }
        }

        private static string GetFinishPositionInfo(Driver driver, bool addStartPosition)
        {
            string endPosition = driver.Finished ? driver.FinishingPosition.ToString() : "DNF";
            if (!addStartPosition)
            {
                return endPosition;
            }

            Lap firstLap = driver.Laps.LastOrDefault(x => x.LapNumber == 1);
            return firstLap?.LapStartSnapshot == null ? endPosition : $"{endPosition} (Started: {firstLap.LapStartSnapshot.PlayerData.Position})";
        }

        private void AddDriversInfoHeader(ExcelWorksheet sheet)
        {
            ExcelStyle style = sheet.Cells[5,1,5,9].Style;
            style.Font.Bold = true;
            style.Font.Size = 14;
            style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells["A5"].Value = "#";
            sheet.Cells["B5"].Value = "Name";
            sheet.Cells["C5"].Value = "Car";
            sheet.Cells["D5"].Value = "Laps";
            sheet.Cells["E5"].Value = "Best Lap";
            sheet.Cells["F5"].Value = "Best S1";
            sheet.Cells["G5"].Value = "Best S2";
            sheet.Cells["H5"].Value = "Best S3";
            sheet.Cells["I5"].Value = "Top Speed";
        }

        private void AddTrackInformation(ExcelWorksheet sheet, SessionSummary sessionSummary)
        {
            StringBuilder trackInformation = new StringBuilder(sessionSummary.SessionType.ToString());
            trackInformation.Append(" at: ");
            trackInformation.Append(sessionSummary.TrackInfo.TrackName);
            if (!string.IsNullOrWhiteSpace(sessionSummary.TrackInfo.TrackLayoutName))
            {
                trackInformation.Append(" (");
                trackInformation.Append(sessionSummary.TrackInfo.TrackLayoutName);
                trackInformation.Append(") (");
                trackInformation.Append(GetSessionLength(sessionSummary));
                trackInformation.Append(")");
            }
            sheet.Cells["A1"].Value = trackInformation.ToString();
            sheet.Cells[1,1,1,9].Merge = true;
            ExcelStyle style = sheet.Cells["A1"].Style;

            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.PatternColor.SetColor(System.Drawing.Color.Black);
            style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
            style.VerticalAlignment = ExcelVerticalAlignment.Center;
            style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            style.Font.Color.SetColor(System.Drawing.Color.White);
            style.Font.Bold = true;
            style.Font.Size = 18;

            sheet.Row(1).Height = 35;

        }

        private void AddSessionBestInfo(SessionSummary sessionSummary, ExcelWorksheet sheet)
        {
            ExcelRange range = sheet.Cells["K5:L9"];
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium, SessionBestColor.ToDrawingColor());
            sheet.Cells["K5"].Value = "Session Best:";
            sheet.Cells["K6"].Value = "Sector 1:";
            sheet.Cells["K7"].Value = "Sector 2:";
            sheet.Cells["K8"].Value = "Sector 3:";
            sheet.Cells["K9"].Value = "Lap:";

            if (sessionSummary.SessionBestSector1 != null)
            {
                sheet.Cells["L6"].Value = FormatSessionBest(
                    sessionSummary.SessionBestSector1,
                    sessionSummary.SessionBestSector1.Sector1);
            }

            if (sessionSummary.SessionBestSector2 != null)
            {
                sheet.Cells["L7"].Value = FormatSessionBest(
                    sessionSummary.SessionBestSector2,
                    sessionSummary.SessionBestSector2.Sector2);
            }

            if (sessionSummary.SessionBestSector3 != null)
            {
                sheet.Cells["L8"].Value = FormatSessionBest(
                    sessionSummary.SessionBestSector3,
                    sessionSummary.SessionBestSector3.Sector3);
            }

            if (sessionSummary.SessionBestLap != null)
            {
                sheet.Cells["L9"].Value = FormatSessionBest(
                    sessionSummary.SessionBestLap,
                    sessionSummary.SessionBestLap.LapTime);
            }
            sheet.Cells["K5:K9"].Style.Font.Bold = true;
            sheet.Cells["L5:L9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            range.AutoFitColumns();

        }

        private string FormatSessionBest(Lap lap, TimeSpan timeSpan)
        {
            if (lap == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder(lap.Driver.DriverName);
            sb.Append("-Lap: ");
            sb.Append(lap.LapNumber);
            sb.Append(" | ");
            sb.Append(FormatTimeSpan(timeSpan));
            return sb.ToString();
        }

        private string GetSessionLength(SessionSummary sessionSummary)
        {
            if (sessionSummary.SessionLengthType == SessionLengthType.Laps)
            {
                return sessionSummary.TotalNumberOfLaps + " Laps";
            }
            if (sessionSummary.SessionLength.Hours > 0)
            {
                return sessionSummary.SessionLength.Hours + "h " + (sessionSummary.SessionLength.Minutes + 1) + "min" ;
            }
            return Math.Ceiling((decimal) sessionSummary.SessionLength.Minutes) + "mins";
        }

        private static void CreateWorkBook(ExcelPackage package, bool includeRaceProgress)
        {
            package.Workbook.Worksheets.Add(SummarySheet);
            package.Workbook.Worksheets.Add(LapsAndSectorsSheet);
            package.Workbook.Worksheets.Add(PlayerLapsSheet);

            if (includeRaceProgress)
            {
                package.Workbook.Worksheets.Add(RaceProgressSheet);
            }
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return "-";
            }

            // String seconds = timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            // String miliseconds = timeSpan.Milliseconds < 10 ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();
            // return timeSpan.Minutes + ":" + timeSpan.Seconds + "." + timeSpan.Milliseconds;
            return timeSpan.ToString("mm\\:ss\\.fff");
        }

    }
}



