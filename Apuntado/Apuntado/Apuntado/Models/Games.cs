namespace Apuntado.Models
{
    
    using global::SQLite;
    using System;

    [Table("Games")]
    public class Games
    {
            [PrimaryKey,AutoIncrement]
            public int IdGame { get; set; }

            [MaxLength(30), Unique]
            public string Name { get; set; }

            public DateTime Date { get; set; }

    }
}
