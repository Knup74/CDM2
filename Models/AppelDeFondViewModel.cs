namespace CDM.Models
{
    public class AppelDeFondViewModel
    {
        public int CoproprietaireId { get; set; }
        public string CoproprietaireNom { get; set; } = string.Empty;
        public decimal MontantDu { get; set; }
        public decimal MontantRegle { get; set; }
        public decimal Regularisation { get; set; }
    }
}