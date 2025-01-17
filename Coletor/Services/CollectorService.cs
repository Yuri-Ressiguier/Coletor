﻿using Coletor.Data;
using Coletor.Models;
using Coletor.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using X.PagedList;


namespace Coletor.Services
{
    public class CollectorService
    {
        private readonly MyDbContext _myDbContext;
        private readonly IWebHostEnvironment _appEnvironment;

        public CollectorService(MyDbContext myDbContext, IWebHostEnvironment appEnvironment)
        {
            _myDbContext = myDbContext;
            _appEnvironment = appEnvironment;
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        public async Task FindAllDocuments(Collector col, string type, bool subfolder)
        {
            IEnumerable<FileInfo> fileList;

            if (subfolder)
            {
                fileList = new DirectoryInfo(col.InputPath).EnumerateFiles("*" + type, SearchOption.AllDirectories)
               .Where(x => !x.DirectoryName
               .Equals(col.OutputPath))
               .ToList();
            }
            else
            {
                fileList = new DirectoryInfo(col.InputPath).EnumerateFiles("*" + type, SearchOption.TopDirectoryOnly)
               .Where(x => !x.DirectoryName
               .Equals(col.OutputPath))
               .ToList();
            }

            foreach (var file in fileList)
            {
                Document doc = new Document { IsHere = true, Name = file.Name, Path = file.DirectoryName, Size = file.Length };
                File.Copy(file.FullName, Path.Combine(col.OutputPath, file.Name), true);
                await _myDbContext.Document.AddAsync(doc);
                col.AddDocs(doc);
                col.NumDocFound += 1;
            }

        }


        public void FindDocumentsFromList(Document doc, Collector col, string type, bool subfolder)
        {
            FileInfo? file;

            if (subfolder)
            {
                file = new DirectoryInfo(col.InputPath).EnumerateFiles(doc.Name, SearchOption.AllDirectories)
               .Where(x => !x.DirectoryName
               .Equals(col.OutputPath))
               .FirstOrDefault();
            }
            else
            {
                file = new DirectoryInfo(col.InputPath).EnumerateFiles(doc.Name, SearchOption.TopDirectoryOnly)
               .Where(x => !x.DirectoryName
               .Equals(col.OutputPath))
               .FirstOrDefault();
            }


            if (file != null)
            {
                File.Copy(file.FullName, Path.Combine(col.OutputPath, file.Name), true);
                doc.IsHere = true;
                doc.Size = file.Length;
                doc.Path = file.FullName;
                col.NumDocFound += 1;
            }
            else
            {
                doc.Path = "";
            }

        }

        //Entrada do controller.
        public async Task<int> RegisterDocumentAsync(SearchFilesViewModel vm)
        {
            Collector col = vm.Collector;
            if (col.InternFile != null)
            {
                var fileName = Path.GetFileName(col.InternFile.FileName);
                var uniqueFileName = GetUniqueFileName(fileName);
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "PendingDocumentsTxt");
                var filePath = Path.Combine(uploads, uniqueFileName);
                col.InternFileName = uniqueFileName;
                col.InternFilePath = filePath;

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await col.InternFile.CopyToAsync(fs);
                    FileInfo existingFile = new FileInfo(filePath);
                    if (existingFile.Exists)
                    {
                        if (!existingFile.Extension.Equals(".xlsx"))
                        {
                            throw new ApplicationException("O arquivo fornecido não é um xlsx");
                        }
                        using (ExcelPackage package = new ExcelPackage(existingFile))
                        {
                            //get the first worksheet in the workbook
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            int colCount = worksheet.Dimension.End.Column;  //get Column Count
                            int rowCount = worksheet.Dimension.End.Row;     //get row count
                            for (int row = 2; row <= rowCount; row++)
                            {
                                for (int colx = 1; colx <= colCount; colx++)
                                {
                                    Document doc = new Document(worksheet.Cells[row, colx].Value.ToString().Trim());
                                    FindDocumentsFromList(doc, col, vm.Type, vm.Subfolder);
                                    await _myDbContext.Document.AddAsync(doc);
                                    col.AddDocs(doc);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException("Arquivo não encontrado", uniqueFileName);
                    }
                }

            }
            else
            {
                FindAllDocuments(col, vm.Type, vm.Subfolder);
            }
            await _myDbContext.Collector.AddAsync(col);
            await _myDbContext.SaveChangesAsync();
            await _myDbContext.Entry(col).GetDatabaseValuesAsync();
            return col.Id;
        }

        public async Task<IPagedList<Collector>> FindAllAsync(int page)
        {
            return await _myDbContext.Collector.OrderBy(x => x.Id).ToPagedListAsync(page, 10);
        }

        public async Task<Collector> FindByIdAsync(int id)
        {
            return await _myDbContext.Collector.Include(x => x.Documents).FirstOrDefaultAsync(x => x.Id == id);
        }

        //FILETYPE
        public async Task<List<FileType>> FindAllFileTypesAsync()
        {
            return await _myDbContext.FileType.ToListAsync();
        }

        public async Task AddFileType(string newFileTypeName)
        {
            await _myDbContext.FileType.AddAsync(new FileType(newFileTypeName));
            await _myDbContext.SaveChangesAsync();
        }

    }
}
