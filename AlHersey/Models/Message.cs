using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlHersey.Models
{
	public class Message
	{

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int MessageID { get; set; }

		public int UserID { get; set; }
		public int ProductID { get; set; }

		[StringLength(150)]
		public string? Content { get; set; }


	}
}
