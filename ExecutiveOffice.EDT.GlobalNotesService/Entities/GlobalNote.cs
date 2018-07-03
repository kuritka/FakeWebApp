using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;

namespace ExecutiveOffice.EDT.GlobalNotesService.Entities
{

    public enum ProcessedState
    {
        New,
        ProcessedByCortana,
        ProcessedByWSS,
        Archived
    }

    public class GlobalNoteAzureEntity : TableEntity
    {

        public GlobalNoteAzureEntity()
        {

        }

        public GlobalNoteAzureEntity(GlobalNote  globalNote)
        {
            GlobalNoteRow =  globalNote != null  ? 
                JsonConvert.SerializeObject(globalNote) 
                : throw new ArgumentNullException($"{nameof(globalNote)}");
        }

        public ProcessedState State { get; set; }

        public string GlobalNoteRow { get; set; }
    }


    public sealed class GlobalNote 
    {

        private readonly DateTime _currentDate;

        public GlobalNote()
        {
            _currentDate = DateTime.Now;
        }


        public string SecurityType { get; set; }

        public string Issuer { get; set; }

        public string LaunchDate { get; set; }

        public string ValueDate { get; set; }

        public string TypeOfOffering { get; set; }

        public string Listing { get; set; }

        public string CBFAccountNumber { get; set; }

        public string Isin { get; set; }

        public string Wkn { get; set; }

        public string OrderNumber { get; set; }

        public string NumberReleased { get; set; }

        public string NumberOfShares { get; set; } 

        public string SecurizationType { get; set; }

        public string Expiry { get; set; }

        public string StartOfExcercisePeriod { get; set; }

        public string EndOfExcercisePeriod { get; set; }

        public string Type { get; set; }

        public string ProductName { get; set; }

        public string PaymentOrShares { get; set; }

        public string DistributionCurrency { get; set; }

        public string PayingAgentAccountNumber { get; set; }

        public string Denomination { get; set; }

        public string MstringransferableAmmount { get; set; }

        public string MinExcercide { get; set; }

        public string ExcerciseAgent { get; set; }

        public string XemacSystemCalimNumber { get; set; }

        public string XemacParticipant { get; set; }

        public string GlobalNoteNumber { get; set; }

        public string AnvendbarEsRecht { get; set; }

        public string Mindestbetrag { get; set; }

        public string Increment{ get; set; }

        public string Language { get; set; }

        public string Comment { get; set; }

        public string Date => $"{_currentDate.ToUniversalTime().ToString("dd. MMMM yyyy")}";

        public string ShortDate => $"{_currentDate.ToUniversalTime().ToString("dd.MM.yyyy")}";

        public string Footer => $"EDT {_currentDate.ToUniversalTime().ToString("u")}";

        public string Venue => "Frankfurt am Main";
    
    }
}
