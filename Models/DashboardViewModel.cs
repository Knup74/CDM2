using System;
using System.Collections.Generic;

namespace CDM.Models
{
    public class DashboardViewModel
    {
        public int CoproprietaireId { get; set; }
        public string CoproprietaireNom { get; set; } = string.Empty;

        // KPI
        public decimal MontantDuCurrentQuarter { get; set; }
        public decimal MontantRegleCurrentQuarter { get; set; }
        public decimal SoldeCumulatif { get; set; } // choix 3:C (toute la dette cumulée)
        public AppelSummaryItem LastAppel { get; set; } = new();

        // Lots
        public List<LotSummary> Lots { get; set; } = new();

        // Historique appels
        public List<AppelSummaryItem> AppelsHistory { get; set; } = new();

        // Graph data
        public List<QuarterSeriesItem> EvolutionSeries { get; set; } = new(); // per trimester of current year
        public List<DonutItem> RepartitionSousCopro { get; set; } = new();
    }

    public class LotSummary
    {
        public int Id { get; set; }
        public string NumeroLot { get; set; } = string.Empty;
        public decimal Tantiemes { get; set; }
        public string SousCopro { get; set; } = string.Empty;
    }

    public class AppelSummaryItem
    {
        public int Id { get; set; }
        public int Annee { get; set; }
        public int TrimestreNumero { get; set; }
        public decimal MontantDu { get; set; }
        public decimal MontantRegle { get; set; }
        public decimal Regularisation { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class QuarterSeriesItem
    {
        public int Numero { get; set; } // 1..4
        public decimal Previsionnel { get; set; }
        public decimal Reel { get; set; }
    }

    public class DonutItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
