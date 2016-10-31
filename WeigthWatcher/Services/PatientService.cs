using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Obalon.Models;
using Obalon.Utils;

namespace Obalon.Services
{

    public interface IPatientService
    {
        ResponseItemList<Patient> GetPatients(int doctorId);

        ResponseItemList<Patient> SearchPatients(); // tb si niste criterii : .. evenimente ?

        ResponseItem<Patient> GetPatient(int pacientId);

        int Add(Patient p);

        string Test();
    }

    public class PatientService : IPatientService
    {
        public PatientService()
        {

        }

        public int Add(Patient patient)
        {
            int returnValue = -1;

            try
            {
                using (var db = new Models.ObalonEntities())
                {
                    if (patient.DoctorId == 0)
                        patient.DoctorId = 77777;

                    db.Patients.Add(patient);

                    db.SaveChanges();

                    returnValue = patient.PatientId;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error while calling GetPatients", ex);
            }

            return returnValue;
        }

        public ResponseItemList<Patient> GetPatients(int doctorId)
        {
            ResponseItemList<Patient> returnValue = new Utils.ResponseItemList<Models.Patient>();

            try
            {
                using (var db = new Models.ObalonEntities())
                {
                    returnValue.Items = db.Patients.Where(p => p.DoctorId == doctorId).OrderByDescending(p => p.PatientId).ToList();
                    returnValue.TotalRecords = returnValue.Items.Count;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error while calling GetPatients", ex);
            }

            return returnValue;
        }

        public ResponseItem<Patient> GetPatient(int pacientId)
        {
            ResponseItem<Patient> returnValue = new ResponseItem<Patient>();

            try
            {
                using (var db = new ObalonEntities())
                {
                    returnValue.Item = db.Patients.Where(p => p.PatientId == pacientId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error while calling GetPatient", ex);
            }

            return returnValue;
        }


        public ResponseItemList<Patient> SearchPatients()
        {
            ResponseItemList<Patient> returnValue = null;

            try
            {

            }
            catch (Exception ex)
            {
                //logger.Error("Error while calling SearchPacients", ex);
            }

            return returnValue;
        }

  

        public string Test()
        {
            return "aloha";
        }
    }
}
