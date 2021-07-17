using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemalto_ID_Scan_Daemon___Claysys.Model
{
    public class DrivingLicense
    {
        public class DriverLicense
        {
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Birthdate { get; set; }
            public string CardRevisionDate { get; set; }
            public string City { get; set; }
            public string ClassificationCode { get; set; }
            public string ComplianceType { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string DocumentType { get; set; }
            public string EndorsementCodeDescription { get; set; }
            public string EndorsementsCode { get; set; }
            public string ExpirationDate { get; set; }
            public string EyeColor { get; set; }
            public string FirstName { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public object HAZMATExpDate { get; set; }
            public string HairColor { get; set; }
            public string Height { get; set; }
            public string IIN { get; set; }
            public string IssueDate { get; set; }
            public string IssuedBy { get; set; }
            public string JurisdictionCode { get; set; }
            public string LastName { get; set; }
            public string LicenseNumber { get; set; }
            public object LimitedDurationDocument { get; set; }
            public string MiddleName { get; set; }
            public string NamePrefix { get; set; }
            public string NameSuffix { get; set; }
            public object OrganDonor { get; set; }
            public string PostalBox { get; set; }
            public string PostalCode { get; set; }
            public string Race { get; set; }
            public string RestrictionCode { get; set; }
            public string RestrictionCodeDescription { get; set; }
            public string VehicleClassCode { get; set; }
            public string VehicleClassCodeDescription { get; set; }
            public object Veteran { get; set; }
            public string WeightKG { get; set; }
            public string WeightLBS { get; set; }
        }

        public class ValidationCode
        {
            public List<object> Errors { get; set; }
            public bool IsValid { get; set; }
        }

        public class ParseImageResult
        {
            public DriverLicense DriverLicense { get; set; }
            public string ErrorMessage { get; set; }
            public string Reference { get; set; }
            public bool Success { get; set; }
            public ValidationCode ValidationCode { get; set; }
        }

        public class Root
        {
            public ParseImageResult ParseImageResult { get; set; }
        }


    }
}

