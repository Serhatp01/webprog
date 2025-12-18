using System.ComponentModel.DataAnnotations;

namespace OdevDeneme1.Models
{
    public class Uye
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        public string Email { get; set; }

        // 🔑 Şifre sadece login için, formda girilecek
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Sifre { get; set; }

        // ❗ Navigation property → Required OLMAZ
        public List<Randevu>? Randevular { get; set; }
    }
}
