using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BinaryFileManager.Data.Models;
using BinaryFileManager.Data.ViewModels.Shared;
using BinaryFileManager.Logging;
using Microsoft.Extensions.Logging;

namespace BinaryFileManager.Data.ViewModels;

public class BookViewModel : ObservableObject
{
    private readonly BookModel _bookModel = new();
    private readonly ILogger _logger = FileLoggerFactory.GetLogger();

    public string BookTypes
    {
        get => GetFormattedBookTypeFlags(_bookModel.BookTypeFlags);
        set
        {
            _bookModel.BookTypeFlags = GetFormattedBookTypeFlags(value);
            OnPropertyChanged(nameof(BookTypes));
        }
    }

    public string BookSize
     {
        get => GetFormattedPageSize(_bookModel.PageSizeFlag);
        set
        {
            _bookModel.PageSizeFlag = GetFormattedPageSize(value);
            OnPropertyChanged(nameof(BookSize));
        }
    }

    public DateTime? PublicationDate
    {
        get => _bookModel.PublicationDate == DateTime.MinValue ? null : _bookModel.PublicationDate;
        set
        {
            _bookModel.PublicationDate = value ?? DateTime.MinValue;
            OnPropertyChanged(nameof(PublicationDate));
        }
    }

    public string Title
    {
        get => _bookModel.Title;
        set
        {
            _bookModel.Title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public ulong? ProducedQuantity
    {
        get => _bookModel.ProducedQuantity == 0 ? null : _bookModel.ProducedQuantity;
        set
        {
            _bookModel.ProducedQuantity = value ?? 0;
            OnPropertyChanged(nameof(ProducedQuantity));
        }
    }

    private static string GetFormattedPageSize(byte pageSizeFlag)
    {
        switch (pageSizeFlag)
        {
            case 0x1:
                return "A5";
            case 0x2:
                return "A4";
            case 0x3:
                return "Letter";
            case 0x4:
                return "Legal";
            default:
                return "";
        }
    }

    private static byte GetFormattedPageSize(string pageSizeName)
    {
        switch (pageSizeName)
        {
            case "A5":
                return 0x1;
            case "A4":
                return 0x2;
            case "Letter":
                return 0x3;
            case "Legal":
                return 0x4;
            default:
                return 0x0; // Domyślna wartość w przypadku braku dopasowania
        }
    }

    private static string GetFormattedBookTypeFlags(uint bookTypeFlags)
    {
        StringBuilder result = new StringBuilder();

        if ((bookTypeFlags & 0x01) != 0)
            result.Append("Archived, ");

        if ((bookTypeFlags & 0x02) != 0)
            result.Append("Grayscale, ");

        if ((bookTypeFlags & 0x04) != 0)
            result.Append("Has Images, ");

        if ((bookTypeFlags & 0x08) != 0)
            result.Append("Hardback, ");

        if ((bookTypeFlags & 0x10) != 0)
            result.Append("Is Bestseller, ");

        // Usunięcie ostatniego przecinka i spacji, jeśli istnieją
        if (result.Length > 2)
            result.Length -= 2;

        return result.ToString();
    }

    private static uint GetFormattedBookTypeFlags(string bookTypeFlags)
    {
        uint result = 0;

        if (bookTypeFlags.Contains("Archived"))
            result |= 0x01;

        if (bookTypeFlags.Contains("Grayscale"))
            result |= 0x02;

        if (bookTypeFlags.Contains("Has Images"))
            result |= 0x04;

        if (bookTypeFlags.Contains("Hardback"))
            result |= 0x08;

        if (bookTypeFlags.Contains("Is Bestseller"))
            result |= 0x10;

        return result;
    }

    public void LoadBookBinaryFile(string filePath)
    {
        using FileStream fs = new FileStream(filePath, FileMode.Open);
        using BinaryReader br = new BinaryReader(fs);
        
        uint bookTypeFlags = br.ReadUInt16();
        byte pageSizeFlag = br.ReadByte();
        int publicationDateSeconds = br.ReadInt32();
        var publicationDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(publicationDateSeconds);

        List<byte> titleBytes = new();
        byte currentByte;
        while ((currentByte = br.ReadByte()) != 0)
            titleBytes.Add(currentByte);

        string title = Encoding.UTF8.GetString(titleBytes.ToArray());
        ulong producedQuantity = br.ReadUInt64();

        // Sprawdzamy czy odczytano maksymalną wartość ulong, co sugeruje brak danych
        if (producedQuantity == ulong.MaxValue)
            producedQuantity = 0; // Przypisanie wartości domyślnej.

        _logger.LogInformation("||||||||||||||| file logs start |||||||||||||||");
        _logger.LogInformation("filePath - " + filePath);
        _logger.LogInformation("bookTypeFlags - " + bookTypeFlags);
        _logger.LogInformation("pageSizeFlag - " + pageSizeFlag);
        _logger.LogInformation("publicationDate - " + publicationDate);
        _logger.LogInformation("title - " + title);
        _logger.LogInformation("producedQuantity - " + producedQuantity);
        _logger.LogInformation("||||||||||||||| file logs end |||||||||||||||");

        _bookModel.BookTypeFlags = bookTypeFlags;
        _bookModel.PageSizeFlag = pageSizeFlag;
        _bookModel.PublicationDate = publicationDate;
        _bookModel.Title = title;
        _bookModel.ProducedQuantity = producedQuantity;
           
        OnPropertyChanged(null);
    }
}
