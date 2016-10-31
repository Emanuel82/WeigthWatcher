//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics;
////using System.IO.Packaging;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using System.Transactions;
//using System.Web;
////using Org.BouncyCastle.OpenSsl;
//using System.IO;
//using System.Threading;
//using System.Globalization;
//using Obalon.Security;

//namespace Obalon.Services
//{
//    public interface IUserService
//    {
//        User CurrentUser { get; }
//        //UserSessionInfo CurrentSession { get; }
//        //Task CheckSessionExists(User user = null);
//        Task<User> Authenticate(string userName, string password);
//        //Task<User> AuthenticateByCert(string serialNumber, string issuer);
//        //Task<User> AuthenticateInternal(string token);

//        //UserContext UserContext { get; }
//        Task Logout();

//        //bool UserIs(params Values.SysEntityType[] entityType);

//        //int? CurrentUserEntityId { get; }
//        //Task CheckQRCodeCertMap(User user);

//        void Login(User user);
//    }

////    public class TheSession
////    {
////        public static readonly Dictionary<string, UserSessionInfo> SESSIONS = new Dictionary<string, UserSessionInfo>();
////        public static string CurrentHttpSessionID { get { return HttpContext.Current.Request.Cookies["_HttpSessionID"]?.Value ?? string.Empty; } }
////        public static UserSessionInfo CurrentSession { get { return SESSIONS.ContainsKey(CurrentHttpSessionID) ? SESSIONS[CurrentHttpSessionID] : null; } }
////        public static User CurrentUser
////        {
////            get
////            {
////#if TRACE
////                Debug.WriteLine(string.Format("CURRENT_USER [{0}] HTTPSESSIONID [{1}] Entity [{2} {3}]",
////                    CurrentSession?.User?.Username,
////                    CurrentHttpSessionID,
////                    CurrentSession?.User?.Entity.EntityName,
////                    CurrentSession?.User?.Entity?.EntityType));
////#endif

////                return CurrentSession?.User;
////            }
////        }

////        public static void RemoveCurrent()
////        {
////            var s = CurrentSession;
////            if (s == null) return;

////            lock (SESSIONS)
////            {
////                // double check
////                if (!SESSIONS.ContainsKey(s.HttpSessionID)) return;
////                SESSIONS.Remove(s.HttpSessionID);
////                var c = HttpContext.Current.Request.Cookies["_HttpSessionID"];
////                if (c != null)
////                    HttpContext.Current.Request.Cookies.Remove("_HttpSessionID");
////            }
////        }

////        public static void EnsureHttpSession()
////        {
////            var c = HttpContext.Current.Request.Cookies["_HttpSessionID"];
////            if (c == null)
////            {
////                c = new HttpCookie("_HttpSessionID", Guid.NewGuid().ToString("N").ToUpper());
////                c.Expires = DateTime.Now.AddYears(1);
////                c.HttpOnly = true;
////                HttpContext.Current.Response.Cookies.Add(c);
////            }
////        }


////        static readonly CultureInfo DEFAULT_CULTURE = new CultureInfo("ro-RO");
////        static readonly List<string> acceptedCultures = new List<string>() { "ro-RO", "en-US" };
////        public static void EnsureCulture()
////        {
////            // set culture by request headers
////            var ci = DEFAULT_CULTURE;
////            var culture = HttpContext.Current.Request.Cookies["culture"]?.Value;
////            if (culture != null && acceptedCultures.Any(x => x == culture))
////                ci = new CultureInfo(culture);

////            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
////        }
////    }

//    public class UserService : IUserService
//    {
//        //private readonly UserSessionRepository _userSessionRepository;
//        //private readonly IAuthenticationRepository _authenticationRepository;
//        //private readonly ILogService _log;
//        private readonly ICacheService _cache;


//        #region Public props

//        public User CurrentUser => TheSession.CurrentUser;
//        public UserSessionInfo CurrentSession => TheSession.CurrentSession;
//        public int? CurrentUserEntityId => TheSession.CurrentUser?.Entity?.EntityID;

//        public Task CheckQRCodeCertMap(User user)
//        {
//            // check if QR code auth need to map certificate with user
//            if (user == null || CurrentSession == null || CurrentSession.QRCertPem == null) return null;

//            //var cert = PemUtils.ReadCert(CurrentSession.QRCertPem);
//            var cert = (Org.BouncyCastle.X509.X509Certificate)new PemReader(new StringReader(CurrentSession.QRCertPem)).ReadObject();
//            if (cert.IsValid(DateTime.Now))
//                try
//                {
//                    _authenticationRepository.MapUserCert(user, cert.SerialNumber.ToString(), cert.IssuerDN.ToString(), cert.SubjectDN.ToString(), cert.NotBefore, cert.NotAfter, CurrentSession.QRCertPem);
//                }
//                catch (Exception ex)
//                {
//                    _log.Error("could not map user account with QR code certificate", ex);   // sad story: can't inform user about this
//                }

//            CurrentSession.QRCertPem = null;

//            return null;
//        }

//        #endregion

//        #region Ctor

//        public UserService(UserSessionRepository userSessionRepository, IAuthenticationRepository authenticationRepository, ILogService log, ICacheService cache)
//        {
//            _userSessionRepository = userSessionRepository;
//            _authenticationRepository = authenticationRepository;
//            _log = log;
//            _cache = cache;
//        }

//        #endregion

//        #region Public methods



//        public async Task CheckSessionExists(User user = null)
//        {
//            try
//            {
//                var httpSessionID = TheSession.CurrentHttpSessionID;

//                Debug.WriteLine("CheckSession() " + httpSessionID);

//                // check if session exists in memory cache
//                if (TheSession.SESSIONS.ContainsKey(httpSessionID)) return;

//                // not in cache -> must add
//                lock (TheSession.SESSIONS)
//                {
//                    // double check
//                    if (TheSession.SESSIONS.ContainsKey(httpSessionID)) return;

//                    // check session exists in db  (probably starded session on WAS1 and now he is on WAS2)
//                    var us = _userSessionRepository.GetByHttpSessionID(httpSessionID).Result;
//                    if (us == null)
//                    {
//                        // create new session
//                        var now = DateTime.Now;
//                        var ctx = HttpContext.Current;
//                        us = new UserSessionInfo
//                        {
//                            UserID = user?.UserId,
//                            LoginDate = user == null ? default(DateTime?) : now,
//                            AccessDate = now,
//                            HttpSessionID = httpSessionID,
//                            ClientOS = ctx.Request.Browser.Platform,
//                            ClientBrowser = ctx.Request.Browser.Id,
//                            IPAddress = GateSafeUtils.GetIP(),
//                            IPAddressGS = ctx.Request.UserHostAddress,
//                            IPAddressWAS = ctx.Server.MachineName
//                        };
//                        us.UserSessionID = _userSessionRepository.CreateSession(us).Result;
//                    }

//                    // fetch user info if session is authenticated
//                    if (us.UserID.HasValue)
//                        us.User = _authenticationRepository.GetUserByID(us.UserID.Value);

//                    // save session in memory cache
//                    TheSession.SESSIONS.Add(TheSession.CurrentHttpSessionID, us);
//                }

//            }
//            catch (Exception ex)
//            {
//                _log.Error("Could check public session", ex);
//            }
//        }


//        public async Task<User> Authenticate(string userName, string password)
//        {
//            try
//            {
//                using (var tx = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    var user = await _authenticationRepository.FindUser(userName, password);

//                    var success = user != null;

//                    Debug.WriteLine("AUTH-USER: " + userName + " success:" + success);

//                    var gateSafeEnabled = ConfigurationManager.AppSettings["certsign:GateSafe:DisableCertValidation"] == null;
//                    if (success && gateSafeEnabled)
//                    {
//                        var headers = HttpContext.Current.Request.Headers;
//                        var hdebug = headers.AllKeys.Select(k => k + "=" + headers[k] + "\n").Aggregate((a, b) => a + " " + b);
//                        Debug.WriteLine("GATESAFE HEADERES =>" + hdebug);

//                        var serial = GateSafeUtils.GetSerial();
//                        var issuer = GateSafeUtils.GetIssuer();

//                        var entityID = await _authenticationRepository.GateSafeEntity(serial, issuer);
//                        success = user.Entity.EntityID == entityID;

//                        if (!success)
//                            user = null;


//                        Debug.WriteLine("AUTH-USER: " + userName + " serial:" + serial + " issuer:" + issuer + " success:" + success);
//                    }


//                    UserSessionInfo session;
//                    if (success)
//                    {
//                        // login on current public session
//                        session = CurrentSession;
//                        session.LoginDate = DateTime.Now;
//                        session.UserID = user.UserId;

//                        await _userSessionRepository.Login(session);
//                    }
//                    else
//                    {
//                        // login failed -> continue with public current session
//                        session = CurrentSession;
//                    }

//                    // Audit login attempt
//                    await _userSessionRepository.AddOperation(new SessionOperation
//                    {
//                        UserSessionID = session.UserSessionID.Value,
//                        RootEntityID = success ? user.UserId : default(int?),
//                        OperationType = success ? Values.SysOperationType.LoginSuccess : Values.SysOperationType.LoginFailed,
//                        OperationDate = DateTime.Now,
//                        OperationDescription = null, // !certAuthSuccess ? "Cert auth failed" : null,
//                        OperationData = null
//                    });

//                    tx.Complete();

//                    return user;
//                }
//            }
//            catch (Exception ex)
//            {
//                _log.ErrorTicket(string.Format("Could not authenticate user: {0}", userName), ex);
//                return null;
//            }
//        }

//        public async Task<User> AuthenticateByCert(string serialNumber, string issuer)
//        {
//            try
//            {
//                using (var tx = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    var user = await _authenticationRepository.FindUserByCert(serialNumber, issuer);
//                    var success = user != null;

//                    UserSessionInfo session;
//                    if (success)
//                    {
//                        // login on current public session
//                        session = CurrentSession;
//                        session.LoginDate = DateTime.Now;
//                        session.UserID = user.UserId;

//                        await _userSessionRepository.Login(session);
//                    }
//                    else
//                    {
//                        // login failed -> continue with public current session
//                        session = CurrentSession;
//                    }

//                    // Audit login attempt
//                    await _userSessionRepository.AddOperation(new SessionOperation
//                    {
//                        UserSessionID = session.UserSessionID.Value,
//                        RootEntityID = success ? user.UserId : default(int?),
//                        OperationType = success ? Values.SysOperationType.LoginSuccess : Values.SysOperationType.LoginFailed,
//                        OperationDate = DateTime.Now,
//                        OperationDescription = null,
//                        OperationData = null
//                    });

//                    tx.Complete();

//                    return user;
//                }
//            }
//            catch (Exception ex)
//            {
//                _log.ErrorTicket(string.Format("Could not authenticate user by serial: {0}", serialNumber), ex);
//                return null;
//            }
//        }

//        public async Task<User> AuthenticateInternal(string token)
//        {
//            try
//            {
//                await CheckSessionExists();

//                using (var tx = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    var user = await _authenticationRepository.FindByToken(token);
//                    var success = user != null;

//                    UserSessionInfo session;
//                    if (success)
//                    {
//                        // login on current public session
//                        session = CurrentSession;
//                        session.LoginDate = DateTime.Now;
//                        session.UserID = user.UserId;

//                        await _userSessionRepository.Login(session);
//                    }
//                    else
//                    {
//                        // login failed -> continue with public current session
//                        session = CurrentSession;
//                    }

//                    //// Audit login attempt
//                    //await _userSessionRepository.AddOperation(new SessionOperation
//                    //{
//                    //    UserSessionID = session.UserSessionID.Value,
//                    //    RootEntityID = success ? user.UserId : default(int?),
//                    //    OperationType = success ? Values.SysOperationType.LoginSuccess : Values.SysOperationType.LoginFailed,
//                    //    OperationDate = DateTime.Now,
//                    //    OperationDescription = null,
//                    //    OperationData = null 
//                    //});

//                    tx.Complete();

//                    return user;
//                }
//            }
//            catch (Exception ex)
//            {
//                _log.ErrorTicket(string.Format("Could not authenticate interal user"), ex);
//                return null;
//            }
//        }

//        private UserContext _userContext;
//        public UserContext UserContext { get { return _userContext ?? (_userContext = new CachedUserContext(_cache, TheSession.CurrentHttpSessionID)); } }

//        public static Dictionary<string, object> _data = new Dictionary<string, object>();
//        public Dictionary<string, object> Data { get { return _data; } }



//        public bool UserIs(params Values.SysEntityType[] entityType)
//        {
//            return CurrentUser != null && entityType.Any(e => CurrentUser.Entity.EntityType == e);
//        }


//        public void Login(User user)
//        {
//            TheSession.RemoveCurrent();         // remove public session cookie
//            TheSession.EnsureHttpSession();     // add private session cookie 
//            CheckSessionExists(user);               // add new session in db
//            CurrentSession.User = user;         // set current session user
//        }

//        public async Task Logout()
//        {
//            try
//            {
//                // check if session exists in memory cache
//                var s = CurrentSession;
//                if (s == null) return;

//                await _userSessionRepository.Logout(s.HttpSessionID);
//                TheSession.RemoveCurrent();

//                // Audit logout attempt
//                await _userSessionRepository.AddOperation(new SessionOperation
//                {
//                    UserSessionID = s.UserSessionID ?? -1,
//                    RootEntityID = s.UserID,
//                    OperationType = Values.SysOperationType.Logout,
//                    OperationDate = DateTime.Now,
//                    OperationDescription = null,
//                    OperationData = null
//                });
//            }
//            catch (Exception ex)
//            {
//                _log.ErrorTicket(string.Format("Could not logout user session: {0}", TheSession.CurrentHttpSessionID), ex);
//            }
//        }

//        #endregion
//    }


//    //public class ClaimsUtils
//    //{
//    //    public static User BuildFromCurrentClaims()
//    //    {
//    //        var i = ClaimsPrincipal.Current.Identities.First(id => !(id is WindowsIdentity));
//    //        var usr = new User
//    //        {
//    //            UserId = Convert.ToInt32(i.Claims.First(x => x.Type == "UserID").Value),
//    //            UserSessionId = Convert.ToInt32(i.Claims.First(x => x.Type == "UserSessionID").Value),
//    //            Username = i.Claims.First(x => x.Type == "Username").Value,
//    //            FullName = i.Claims.First(x => x.Type == "FullName").Value,
//    //            Entity = new EntityInfo
//    //            {
//    //                EntityID = Convert.ToInt32(i.Claims.First(x => x.Type == "EntityID").Value),
//    //                EntityName = i.Claims.First(x => x.Type == "EntityName").Value,
//    //                EntityType = (Values.SysEntityType)Enum.Parse(typeof(Values.SysEntityType), i.Claims.First(x => x.Type == "EntityType").Value)
//    //            },
//    //            Rights = i.Claims
//    //                        .Where(x => x.Type == typeof(Values.SysAppRight).Name)
//    //                        .Select(x => (Values.SysAppRight)Enum.Parse(typeof(Values.SysAppRight), x.Value))
//    //                        .ToList()
//    //        };

//    //        return usr;
//    //    }
//    //}

//    //public class UserContextParameters
//    //{
//    //    public const string AutoAssignUser = "AutoAssignUser";
//    //}

//    //public class CachedUserContext : UserContext
//    //{
//    //    readonly ICacheService _cache;
//    //    readonly string _httpSessionID;

//    //    public CachedUserContext(ICacheService cache, string httpSessionID)
//    //    {
//    //        _cache = cache;
//    //        _httpSessionID = httpSessionID;
//    //    }

//    //    public override object this[string key]
//    //    {
//    //        get { return _cache[string.Format("UserContext[{0}][{1}]", _httpSessionID, key)]; }
//    //        set { _cache[string.Format("UserContext[{0}][{1}]", _httpSessionID, key)] = value; }
//    //    }
//    //}

//    //public class MockUserContext : UserContext
//    //{
//    //    static readonly Dictionary<string, object> data = new Dictionary<string, object>();
//    //    public override object this[string key]
//    //    {
//    //        get { return data[key]; }
//    //        set { data[key] = value; }
//    //    }
//    //}
//}
