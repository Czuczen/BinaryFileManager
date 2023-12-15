using System;

namespace BinaryFileManager.Data.Models;

public class BookModel
{
    public uint BookTypeFlags { get; set; }

    public byte PageSizeFlag { get; set; }

    public DateTime PublicationDate { get; set; }

    public string Title { get; set; }

    public ulong ProducedQuantity { get; set; }
}