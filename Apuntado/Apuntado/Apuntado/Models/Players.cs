namespace Apuntado.Models
{
    using global::SQLite;
    using System;

    [Table("Players")]
    public class Players
    {
        [PrimaryKey , AutoIncrement]
        public int IdPLayer { get; set; }

        public int IdGame { get; set; }
        
        [MaxLength(50)]
        public string Namep { get; set; }

        public int Points { get; set; }
    }
}
