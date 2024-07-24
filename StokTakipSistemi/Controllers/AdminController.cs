using OfficeOpenXml;
using StokTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StokTakipSistemi.Controllers
{
    
    public class AdminController : Controller
    {
        // GET: Admin
        StokTakipDBEntities db = new StokTakipDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Login(KULLANICI user)
        {
            if (string.IsNullOrEmpty(user.KUL_USERNAME) || string.IsNullOrEmpty(user.KUL_SIFRE))
            {
                ViewBag.ErrorMessage = "Kullanıcı Bilgileri Boş Bırakılamaz";
                return View();
            }

            KULLANICI users = db.KULLANICI.FirstOrDefault(u => u.KUL_USERNAME == user.KUL_USERNAME && u.KUL_SIFRE == user.KUL_SIFRE && u.KUL_TIP == 1);

            if (users != null)
            {
                FormsAuthentication.SetAuthCookie(user.KUL_USERNAME, false);
                Session["Username"] = users.KUL_USERNAME.ToString();
                Session["KulTIP"] = users.KUL_TIP.ToString();
                Session["KulID"] = users.KUL_ID.ToString();
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı Bilgileri yanlış girildi.";
            }

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult KullaniciSayfasi()
        {
            var userList = db.KULLANICI.ToList();
            TempData["Kullanicilar"] = userList;
            TempData["SuccessMessage"] = "Kullanıcı Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Kullanıcı Listesi yüklenirken bir sorun oluştu!";

            return View();
        }
        [HttpGet]
        public ActionResult DepoSayfasi()
        {
            var depoList = db.DEPO.ToList();
            TempData["Depolar"] = depoList;
            TempData["SuccessMessage"] = "Depo listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Depo listesi yüklenirken bir sorun oluştu!";

            return View();
        }

        public ActionResult AltDepoSayfasi()
        {
            var depoList = db.ALT_DEPO.ToList();
            TempData["Alt_Depolar"] = depoList;
            TempData["SuccessMessage"] = "Alt Depo listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Alt Depo listesi yüklenirken bir sorun oluştu!";

            return View();
        }
        public ActionResult DepoEslestirmeSayfasi()
        {
            var depoeşList = db.DEPO_ESLESTIRME.ToList();
            TempData["depoEşleştirme"] = depoeşList;
            TempData["SuccessMessage"] = "Kullanıcı Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Kullanıcı Listesi yüklenirken bir sorun oluştu!";

            return View();
        }
        
        public ActionResult StokSayfasi()
        {
            var stokList = db.STOK.ToList();
            TempData["stoklar"] = stokList;
            TempData["SuccessMessage"] = "Stok Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Stok Listesi yüklenirken bir sorun oluştu!";
            return View();
        }

        public ActionResult ExportToExcelDepo()
        {
            // Veritabanından kullanıcı listesini al
            List<DEPO> depoList = db.DEPO.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Depolar");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Ad";
            workSheet.Cells[1, 2].Value = "Statu";
            workSheet.Cells[1, 3].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 4].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 5].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 6].Value = "Güncelleme Tarihi";


            // Veri satırlarını ekle
            int row = 2;
            foreach (var depo in depoList)
            {
                workSheet.Cells[row, 1].Value = depo.DEPO_ADI;
                workSheet.Cells[row, 2].Value = depo.STATU;
                workSheet.Cells[row, 3].Value = depo.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 4].Value = depo.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 5].Value = depo.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 6].Value = depo.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"DepoList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public ActionResult ExportToExcelAlt_Depo()
        {
            // Veritabanından kullanıcı listesini al
            List<ALT_DEPO> depoList = db.ALT_DEPO.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Alt Depolar");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Ad";
            workSheet.Cells[1, 2].Value = "Statu";
            workSheet.Cells[1, 3].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 4].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 5].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 6].Value = "Güncelleme Tarihi";


            // Veri satırlarını ekle
            int row = 2;
            foreach (var depo in depoList)
            {
                workSheet.Cells[row, 1].Value = depo.ALT_DEPO_ADI;
                workSheet.Cells[row, 2].Value = depo.STATU;
                workSheet.Cells[row, 3].Value = depo.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 4].Value = depo.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 5].Value = depo.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 6].Value = depo.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"AltDepoList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public ActionResult ExportToExcelDepoEs()
        {
            // Veritabanından kullanıcı listesini al
            List<DEPO_ESLESTIRME> depoEsList = db.DEPO_ESLESTIRME.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Alt Depolar");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Depo Adı";
            workSheet.Cells[1, 1].Value = "Alt Depo Adı";
            workSheet.Cells[1, 2].Value = "Statu";
            workSheet.Cells[1, 3].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 4].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 5].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 6].Value = "Güncelleme Tarihi";


            // Veri satırlarını ekle
            int row = 2;
            foreach (var depo in depoEsList)
            {
                workSheet.Cells[row, 1].Value = depo.DEPO.DEPO_ADI;
                workSheet.Cells[row, 1].Value = depo.ALT_DEPO.ALT_DEPO_ADI;
                workSheet.Cells[row, 2].Value = depo.STATU;
                workSheet.Cells[row, 3].Value = depo.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 4].Value = depo.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 5].Value = depo.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 6].Value = depo.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"DepoEşleştirmeList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public ActionResult ExportToExcel()
        {
            // Veritabanından kullanıcı listesini al
            List<KULLANICI> userList = db.KULLANICI.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Users");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Username";
            workSheet.Cells[1, 2].Value = "Ad";
            workSheet.Cells[1, 3].Value = "Soyad";
            workSheet.Cells[1, 4].Value = "Tip";
            workSheet.Cells[1, 5].Value = "Statu";
            workSheet.Cells[1, 6].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 7].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 8].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 9].Value = "Güncelleme Tarihi";

            // Veri satırlarını ekle
            int row = 2;
            foreach (var user in userList)
            {
                workSheet.Cells[row, 1].Value = user.KUL_USERNAME;
                workSheet.Cells[row, 2].Value = user.KUL_AD;
                workSheet.Cells[row, 3].Value = user.KUL_SOYAD;
                workSheet.Cells[row, 4].Value = user.KUL_TIP;
                workSheet.Cells[row, 5].Value = user.STATU;
                workSheet.Cells[row, 6].Value = user.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 7].Value = user.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 8].Value = user.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 9].Value = user.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"UserList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public ActionResult ExportToExcelStok()
        {
            // Veritabanından kullanıcı listesini al
            List<STOK> stokList = db.STOK.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Stoklar");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Stok Adı";
            workSheet.Cells[1, 2].Value = "Stok Ölçübirimi";
            workSheet.Cells[1, 3].Value = "Stok Markası";
            workSheet.Cells[1, 4].Value = "Stok Detayı";
            workSheet.Cells[1, 5].Value = "Kayıt Tarihi";
            workSheet.Cells[1, 6].Value = "Min Miktar";
            workSheet.Cells[1, 7].Value = "Statu";
            workSheet.Cells[1, 8].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 9].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 10].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 11].Value = "Güncelleme Tarihi";

            // Veri satırlarını ekle
            int row = 2;
            foreach (var stok in stokList)
            {
                workSheet.Cells[row, 1].Value = stok.STOK_AD;
                workSheet.Cells[row, 2].Value = stok.STOK_OLCUBIRIM;
                workSheet.Cells[row, 3].Value = stok.STOK_MARKA;
                workSheet.Cells[row, 4].Value = stok.STOK_DETAY;
                workSheet.Cells[row, 5].Value = stok.KAYIT_TARIHI;
                workSheet.Cells[row, 6].Value = stok.MIN_MIKTAR;
                workSheet.Cells[row, 7].Value = stok.STATU;
                workSheet.Cells[row, 8].Value = stok.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 9].Value = stok.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 10].Value = stok.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 11].Value = stok.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"StokList{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public ActionResult KullaniciEkle(KULLANICI user)
        {
            if (string.IsNullOrEmpty(user.KUL_USERNAME) || string.IsNullOrEmpty(user.KUL_SIFRE) || string.IsNullOrEmpty(user.KUL_AD) || string.IsNullOrEmpty(user.KUL_SOYAD)
                || string.IsNullOrEmpty(user.KUL_TIP.ToString()) || string.IsNullOrEmpty(user.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Kullanıcı Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingUser = db.KULLANICI.FirstOrDefault(u => u.KUL_USERNAME == user.KUL_USERNAME);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Bu kullanıcı adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    var newUser = new KULLANICI { GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]), GUNCELLEME_TARIHI = DateTime.Now, OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]), OLUSTURMA_TARIHI = DateTime.Now, KUL_USERNAME = user.KUL_USERNAME, KUL_SIFRE = user.KUL_SIFRE, KUL_AD = user.KUL_AD, KUL_SOYAD = user.KUL_SOYAD, KUL_TIP = user.KUL_TIP, STATU = user.STATU };

                    db.KULLANICI.Add(newUser);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("KullaniciSayfasi");
        }


        public ActionResult KullanıcıSil(int user)
        {
            var us = db.KULLANICI.Find(user);
            db.KULLANICI.Remove(us);
            db.SaveChanges();
            return RedirectToAction("KullaniciSayfasi");
        }
        public ActionResult DepoSil(int depo)
        {
            var Depo = db.DEPO.Find(depo);
            db.DEPO.Remove(Depo);
            db.SaveChanges();
            return RedirectToAction("DepoSayfasi");
        }

        public ActionResult AltDepoSil(int altdepo)
        {
            var altDepo = db.ALT_DEPO.Find(altdepo);
            db.ALT_DEPO.Remove(altDepo);
            db.SaveChanges();
            return RedirectToAction("AltDepoSayfasi");
        }   
        public ActionResult DepoEslestirmeSil(int depoEs)
        {
            var depoE = db.DEPO_ESLESTIRME.Find(depoEs);
            db.DEPO_ESLESTIRME.Remove(depoE);
            db.SaveChanges();
            return RedirectToAction("DepoEslestirmeSayfasi");
        }
        public ActionResult StokSil(int stokID)
        {
            var stok = db.STOK.Find(stokID);
            db.STOK.Remove(stok);
            db.SaveChanges();
            return RedirectToAction("StokSayfasi");
        }

        [HttpGet]
        public ActionResult KullanıcıGüncelle(int id)
        {
            var currentUser = db.KULLANICI.Find(id);

            return View(currentUser);
        }
        [HttpGet]
        public ActionResult DepoGüncelle(int id)
        {
            var currentDepo = db.DEPO.Find(id);

            return View(currentDepo);
        }

        [HttpGet]
        public ActionResult AltDepoGüncelle(int id)
        {
            var currentDepo = db.ALT_DEPO.Find(id);

            return View(currentDepo);
        }

        [HttpGet]
        public ActionResult DepoEsGüncelle(int id)
        {
            var currentDepoEs = db.DEPO_ESLESTIRME.Find(id);

            return View(currentDepoEs);
        }

        [HttpGet]
        public ActionResult StokGuncelle(int id)
        {
            var currentStok = db.STOK.Find(id);

            return View(currentStok);
        }

        [HttpPost]
        public ActionResult KullanıcıGüncelle(int id, KULLANICI userNow)
        {


            var currentUser = db.KULLANICI.Find(id);
            currentUser.KUL_USERNAME = userNow.KUL_USERNAME;
            currentUser.KUL_AD = userNow.KUL_AD;
            currentUser.KUL_SOYAD = userNow.KUL_SOYAD;
            currentUser.KUL_SIFRE = userNow.KUL_SIFRE;
            currentUser.KUL_TIP = userNow.KUL_TIP;
            currentUser.KUL_SIFRE = userNow.KUL_SIFRE;
            currentUser.STATU = userNow.STATU;

            if (string.IsNullOrEmpty(userNow.KUL_USERNAME) || string.IsNullOrEmpty(userNow.KUL_SIFRE) || string.IsNullOrEmpty(userNow.KUL_AD) || string.IsNullOrEmpty(userNow.KUL_SOYAD)
              || string.IsNullOrEmpty(userNow.KUL_TIP.ToString()) || string.IsNullOrEmpty(userNow.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Kullanıcı Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingUser = db.KULLANICI.FirstOrDefault(u => u.KUL_USERNAME == userNow.KUL_USERNAME);
                if (existingUser != null && existingUser.KUL_USERNAME != currentUser.KUL_USERNAME)
                {
                    ViewBag.ErrorMessage = "Bu kullanıcı adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    userNow.OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    userNow.OLUSTURMA_TARIHI = DateTime.Now;
                    userNow.GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    userNow.GUNCELLEME_TARIHI = DateTime.Now;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("KullaniciSayfasi");
        }
       



        public ActionResult DepoEkle(DEPO depo)
        {
            if (string.IsNullOrEmpty(depo.DEPO_ADI) || string.IsNullOrEmpty(depo.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Depo Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingDepo = db.DEPO.FirstOrDefault(u => u.DEPO_ADI == depo.DEPO_ADI);
                if (existingDepo != null)
                {
                    ViewBag.ErrorMessage = "Bu depo adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    var newDepo = new DEPO { GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]), GUNCELLEME_TARIHI = DateTime.Now, OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]), OLUSTURMA_TARIHI = DateTime.Now, DEPO_ADI = depo.DEPO_ADI, STATU = depo.STATU };

                    db.DEPO.Add(newDepo);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("DepoSayfasi");
        }

        public ActionResult StokEkle(STOK stok)
        {
            if (string.IsNullOrEmpty(stok.STOK_AD) || string.IsNullOrEmpty(stok.STOK_OLCUBIRIM.ToString()) || string.IsNullOrEmpty(stok.STOK_MARKA.ToString()) || string.IsNullOrEmpty(stok.STOK_DETAY.ToString()) || string.IsNullOrEmpty(stok.KAYIT_TARIHI.ToString()) || string.IsNullOrEmpty(stok.MIN_MIKTAR.ToString()) || string.IsNullOrEmpty(stok.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Stok Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingStok = db.STOK.FirstOrDefault(u => u.STOK_AD == stok.STOK_AD);
                if (existingStok != null)
                {
                    ViewBag.ErrorMessage = "Bu stok adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    var newStok = new STOK { STOK_OLCUBIRIM = stok.STOK_OLCUBIRIM, STOK_MARKA = stok.STOK_MARKA, STOK_DETAY = stok.STOK_DETAY, KAYIT_TARIHI = stok.KAYIT_TARIHI, MIN_MIKTAR = stok.MIN_MIKTAR, GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]), GUNCELLEME_TARIHI = DateTime.Now, OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]), OLUSTURMA_TARIHI = DateTime.Now, STOK_AD = stok.STOK_AD, STATU = stok.STATU };

                    db.STOK.Add(newStok);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("StokSayfasi");
        }

        public ActionResult AltDepoEkle()
        {
            
            var depolar = db.DEPO.ToList();

            ViewBag.DepoListesi = new SelectList(depolar, "DEPO_ID", "DEPO_ADI");

            return View();
        }


        [HttpPost]
        public ActionResult AltDepoKaydet(ALT_DEPO altDepo, int DEPO_ID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(altDepo.ALT_DEPO_ADI) || string.IsNullOrEmpty(altDepo.STATU.ToString())|| string.IsNullOrEmpty(DEPO_ID.ToString()))
                    {
                        ViewBag.ErrorMessage = "Alt Depo Bilgileri Boş Bırakılamaz";
                        return RedirectToAction("AltDepoEkle");
                    }
                    else
                    {
                        altDepo.OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                        altDepo.OLUSTURMA_TARIHI = DateTime.Now;
                        altDepo.GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                        altDepo.GUNCELLEME_TARIHI = DateTime.Now;

                        db.ALT_DEPO.Add(altDepo);
                        db.SaveChanges();


                        DEPO_ESLESTIRME yeniEslesme = new DEPO_ESLESTIRME
                        {
                            DEPO_ID = DEPO_ID, // Seçilen depo ID'si
                            ALT_DEPO_ID = altDepo.ALT_DEPO_ID,
                            STATU = altDepo.STATU,
                            OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]),
                            OLUSTURMA_TARIHI = DateTime.Now,
                            GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]),
                            GUNCELLEME_TARIHI = DateTime.Now
                        };

                        db.DEPO_ESLESTIRME.Add(yeniEslesme);
                        db.SaveChanges();

                        TempData["SuccessMessage"] = "Alt Depo başarıyla eklendi ve Depo Eşleştirme yapıldı.";
                        return RedirectToAction("AltDepoSayfasi");
                    }

                    
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Alt Depo eklenirken bir hata oluştu: " + ex.Message;
                }
            }

            var depolar = db.DEPO.ToList();
            ViewBag.DepoListesi = new SelectList(depolar, "DEPO_ID", "DEPO_ADI");

            return View("AltDepoEkle", altDepo);
        }


        [HttpPost]
        public ActionResult DepoGüncelle(int id, DEPO depoNow)
        {


            var currentDepo = db.DEPO.Find(id);
            currentDepo.DEPO_ADI = depoNow.DEPO_ADI;
            currentDepo.STATU = depoNow.STATU;

            if (string.IsNullOrEmpty(depoNow.DEPO_ADI) || string.IsNullOrEmpty(depoNow.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Depo Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingDepo = db.DEPO.FirstOrDefault(u => u.DEPO_ADI == depoNow.DEPO_ADI);
                if (existingDepo != null && existingDepo.DEPO_ADI != currentDepo.DEPO_ADI)
                {
                    ViewBag.ErrorMessage = "Bu depo adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    depoNow.OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    depoNow.OLUSTURMA_TARIHI = DateTime.Now;
                    depoNow.GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    depoNow.GUNCELLEME_TARIHI = DateTime.Now;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("DepoSayfasi");
        }

        [HttpPost]
        public ActionResult AltDepoGüncelle(int id, ALT_DEPO depoNow)
        {


            var currentDepo = db.ALT_DEPO.Find(id);
            currentDepo.ALT_DEPO_ADI = depoNow.ALT_DEPO_ADI;
            currentDepo.STATU = depoNow.STATU;

            if (string.IsNullOrEmpty(depoNow.ALT_DEPO_ADI) || string.IsNullOrEmpty(depoNow.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Alt Depo Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingDepo = db.ALT_DEPO.FirstOrDefault(u => u.ALT_DEPO_ADI == depoNow.ALT_DEPO_ADI);
                
                if (existingDepo != null && existingDepo.ALT_DEPO_ADI != currentDepo.ALT_DEPO_ADI)
                {
                    ViewBag.ErrorMessage = "Bu alt depo adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    depoNow.OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    depoNow.OLUSTURMA_TARIHI = DateTime.Now;
                    depoNow.GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    depoNow.GUNCELLEME_TARIHI = DateTime.Now;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("AltDepoSayfasi");
        }

        [HttpPost]
        public ActionResult DepoEsGüncelle(int id, DEPO_ESLESTIRME depoNow)
        {

            var currentDepoEslesme = db.DEPO_ESLESTIRME.Find(id);
            if (string.IsNullOrEmpty(depoNow.DEPO.DEPO_ADI.ToString()) || string.IsNullOrEmpty(depoNow.STATU.ToString()) || string.IsNullOrEmpty(depoNow.ALT_DEPO.ALT_DEPO_ADI.ToString()))
            {
                if (depoNow.STATU.ToString().ToLower() != "false" || (depoNow.STATU.ToString().ToLower() != "true")){
                    ViewBag.ErrorMessage = "Statu Bilgisi Hatalıdır";
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = "Depo Bilgileri Boş Bırakılamaz";
                    return View();
                }
                
            }
            else
            {
                    var existingAltDepo = db.ALT_DEPO.FirstOrDefault(u => u.ALT_DEPO_ADI == depoNow.ALT_DEPO.ALT_DEPO_ADI);
                    var existingDepo = db.DEPO.FirstOrDefault(u => u.DEPO_ADI == depoNow.DEPO.DEPO_ADI);
                    if (existingAltDepo != null && existingDepo != null)
                    {
                        
                        currentDepoEslesme.DEPO.DEPO_ADI = depoNow.DEPO.DEPO_ADI;
                        currentDepoEslesme.ALT_DEPO.ALT_DEPO_ADI = depoNow.ALT_DEPO.ALT_DEPO_ADI;
                        currentDepoEslesme.STATU = depoNow.STATU;
                        depoNow.OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                        depoNow.OLUSTURMA_TARIHI = DateTime.Now;
                        depoNow.GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                        depoNow.GUNCELLEME_TARIHI = DateTime.Now;
                        depoNow.DEPO.DEPO_ADI = existingDepo.DEPO_ADI;
                        depoNow.ALT_DEPO.ALT_DEPO_ADI = existingAltDepo.ALT_DEPO_ADI;
                        depoNow.ALT_DEPO_ID = existingAltDepo.ALT_DEPO_ID;
                        depoNow.DEPO_ID = existingDepo.DEPO_ID;
                        db.SaveChanges();

                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Depo Bilgileri Hatalıdır1";
                        return View();

                    }

            }
            return RedirectToAction("DepoEslestirmeSayfasi");
        }


        [HttpPost]
        public ActionResult StokGuncelle(int id, STOK stokNow)
        {


            var currentUser = db.STOK.Find(id);
            
            currentUser.STOK_AD = stokNow.STOK_AD;
            currentUser.STOK_OLCUBIRIM = stokNow.STOK_OLCUBIRIM;
            currentUser.STOK_MARKA = stokNow.STOK_MARKA;
            currentUser.STOK_DETAY = stokNow.STOK_DETAY;
            currentUser.KAYIT_TARIHI = stokNow.KAYIT_TARIHI;
            currentUser.MIN_MIKTAR = stokNow.MIN_MIKTAR;
            currentUser.STATU = stokNow.STATU;

            if (string.IsNullOrEmpty(stokNow.STOK_AD) || string.IsNullOrEmpty(stokNow.STOK_OLCUBIRIM.ToString()) || string.IsNullOrEmpty(stokNow.STOK_DETAY) || string.IsNullOrEmpty(stokNow.STOK_MARKA)
              || string.IsNullOrEmpty(stokNow.KAYIT_TARIHI.ToString()) || string.IsNullOrEmpty(stokNow.MIN_MIKTAR.ToString()) || string.IsNullOrEmpty(stokNow.STATU.ToString()))
            {
                ViewBag.ErrorMessage = "Stok Bilgileri Boş Bırakılamaz";
                return View();
            }
            else
            {
                var existingUser = db.STOK.FirstOrDefault(u => u.STOK_AD == stokNow.STOK_AD);
                if (existingUser != null && existingUser.STOK_AD != currentUser.STOK_AD)
                {
                    ViewBag.ErrorMessage = "Bu stok adı kullanılmaktadır!";
                    return View();
                }
                else
                {
                    stokNow.OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    stokNow.OLUSTURMA_TARIHI = DateTime.Now;
                    stokNow.GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]);
                    stokNow.GUNCELLEME_TARIHI = DateTime.Now;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("StokSayfasi");
        }
    }
}