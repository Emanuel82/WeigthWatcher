using Obalon.Models;
using Obalon.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Obalon.Services
{
    public interface IEventService
    {
        ResponseItemList<Event> GetEvents(int patientId);

        ResponseItemList<Event> SearchEvents(); // tb si niste criterii : .. evenimente ?

        bool Save(Event evt);
        // ResponseItem<Event> GetEvent(int pacientId);

        List<SelectListItem> AvailableEventTypes(int patientId);

        int Add(Event p);

    }

    public class EventService : IEventService
    {
        public List<SelectListItem> AvailableEventTypes(int patientId)
        {
            List<SelectListItem> eventsSelect = new List<SelectListItem>();
            List<EventType> eventTypes = new List<EventType>();
            try
            {
                using (var db = new ObalonEntities())
                {
                    Patient patient = db.Patients.Find(patientId);

                    eventTypes.AddRange(from ev in db.EventTypes where ev.IsRoutineAction == true select ev);

                    if (patient != null && patient.Events.Count > 0 && patient.LastUserEventType > 0)
                    {
                        eventTypes.Add((from evt in db.EventTypes
                                        where evt.EventTypeId != patient.LastUserEventType
                                        && evt.IsRoutineAction == false
                                        orderby evt.EventTypeId
                                        select evt)
                                            .FirstOrDefault());
                    }
                    else
                    {
                        var availableEventType = (from evt in db.EventTypes
                                                  where evt.IsRoutineAction == false
                                                  orderby evt.EventTypeId ascending
                                                  select evt).FirstOrDefault();
                        if (availableEventType != null)
                            eventTypes.Add(availableEventType);
                    }
                }


                if (eventTypes.Count > 0)
                {
                    foreach (var evt in eventTypes.OrderBy(ev => ev.EventTypeId).ToList())
                        eventsSelect.Add(new SelectListItem() { Text = evt.EventTypeName, Value = evt.EventTypeId.ToString() });

                }
            }
            catch (Exception x) { }

            return eventsSelect;
        }

        public bool Save(Event evt)
        {
            try
            {
                using (var db = new Models.ObalonEntities())
                {
                    db.Events.Add(evt);
                    db.SaveChanges();
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public int Add(Event e)
        {
            throw new NotImplementedException();
        }

        public ResponseItemList<Event> GetEvents(int patientId)
        {
            ResponseItemList<Event> returnValue = new Utils.ResponseItemList<Event>();

            try
            {
                using (var db = new Models.ObalonEntities())
                {
                    
                    returnValue.Items = db.Events.Include("EventType").Where(ev => ev.PatientId == patientId).OrderByDescending(ev => ev.EventId).ToList(); 
                    // (from ev in db.Events where ev.PatientId == pacientId select ev).ToList();
                    returnValue.TotalRecords = returnValue.Items.Count;
                   
                }
            }
            catch (System.Exception ex) { }

            return returnValue;
        }
        

        public ResponseItemList<Event> SearchEvents()
        {
            throw new NotImplementedException();
        }

    }
}