using OfficeOpenXml;
using StokTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;

namespace StokTakipSistemi.Controllers
{
    public class DepoYetkilisiController : Controller
    {
        StokTakipDBEntities db = new StokTakipDBEntities();
        // GET: DepoYetkilisi
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login() {
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

            KULLANICI users = db.KULLANICI.FirstOrDefault(u => u.KUL_USERNAME == user.KUL_USERNAME && u.KUL_SIFRE == user.KUL_SIFRE && u.KUL_TIP == 2);
          
          
            if (users != null)
            {
                FormsAuthentication.SetAuthCookie(user.KUL_USERNAME, false);
                Session["Username"] = users.KUL_USERNAME;
                Session["KulTIP"] = users.KUL_TIP;
                Session["KulID"] = users.KUL_ID;
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
        public ActionResult StokHareketiSayfasi()
        {
            var hareketList = db.STOK_HAREKET.ToList();
            TempData["stokHareketleri"] = hareketList;
            TempData["SuccessMessage"] = "Stok Hareket Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Stok Hareket Listesi yüklenirken bir sorun oluştu!";
            return View();
        }

       
        public ActionResult StokHareketiEkle()
        {
            var stoklar = db.STOK.ToList();
            ViewBag.stoklar = new SelectList(stoklar, "STOK_ID", "STOK_AD");

            var eslesmeler = db.DEPO_ESLESTIRME.ToList();
            ViewBag.eslesmeler = new SelectList(eslesmeler, "DEPO_ESLESTIRME_ID", "DEPO_ESLESTIRME_ID");

            var sorumlular = db.SORUMLU.ToList();
            ViewBag.sorumlular = new SelectList(sorumlular, "SORUMLU_ID", "SORUMLU_ADI");

            return View();
        }
        [HttpPost]
        public ActionResult StokHareketiKaydet(STOK_HAREKET hareket, int STOK_ID, int DEPO_ESLESTIRME_ID, int SORUMLU_ID)
        {
            if (string.IsNullOrEmpty(hareket.STOK_ID.ToString()) || string.IsNullOrEmpty(hareket.DEPO_ESLESTIRME_ID.ToString()) || string.IsNullOrEmpty(hareket.SORUMLU_ID.ToString())
                || string.IsNullOrEmpty(hareket.HAREKET_TIP.ToString())||string.IsNullOrEmpty(hareket.ACIKLAMA)||string.IsNullOrEmpty(hareket.HAREKET_MIKTAR.ToString())||string.IsNullOrEmpty(hareket.HAREKET_TARIHI.ToString()))
            {
                TempData["ErrorMessage"] = "Stok Hareketi Bilgileri Boş Bırakılamaz";
                return RedirectToAction("StokHareketiEkle");
            }
            else
            {
                
                var newHareket = new STOK_HAREKET { GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]), GUNCELLEME_TARIHI = DateTime.Now, OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]), OLUSTURMA_TARIHI = DateTime.Now, STOK_ID = STOK_ID, DEPO_ESLESTIRME_ID = DEPO_ESLESTIRME_ID,
                    SORUMLU_ID = SORUMLU_ID, HAREKET_TIP = hareket.HAREKET_TIP, ACIKLAMA = hareket.ACIKLAMA, HAREKET_MIKTAR = hareket.HAREKET_MIKTAR = Decimal.Parse(hareket.HAREKET_MIKTAR.ToString().Replace('.', ',')), HAREKET_TARIHI = hareket.HAREKET_TARIHI};

                    db.STOK_HAREKET.Add(newHareket);
                    db.SaveChanges();
                }

            return RedirectToAction("StokHareketiSayfasi");
        }

        public ActionResult StokHareketSil(int stock)
        {
            var st = db.STOK_HAREKET.Find(stock);
            db.STOK_HAREKET.Remove(st);
            db.SaveChanges();
            return RedirectToAction("StokHareketiSayfasi");
        }

        [HttpGet]
        public ActionResult StokHareketGuncelle(int id)
        {
            var currentStokH = db.STOK_HAREKET.Find(id);

            var stoklar = db.STOK.ToList();
            ViewBag.stoklar = new SelectList(stoklar, "STOK_ID", "STOK_AD", currentStokH.STOK_ID);

            var eslesmeler = db.DEPO_ESLESTIRME.ToList();
            ViewBag.eslesmeler = new SelectList(eslesmeler, "DEPO_ESLESTIRME_ID", "DEPO_ESLESTIRME_ID", currentStokH.DEPO_ESLESTIRME_ID);

            var sorumlular = db.SORUMLU.ToList();
            ViewBag.sorumlular = new SelectList(sorumlular, "SORUMLU_ID", "SORUMLU_ADI", currentStokH.SORUMLU_ID);

            return View(currentStokH);
        }

        [HttpPost]
        public ActionResult StokHareketGuncelle(int id, STOK_HAREKET stokNow)
        {


            var currentStokH = db.STOK_HAREKET.Find(id);

            currentStokH.STOK_ID = stokNow.STOK_ID;
            currentStokH.DEPO_ESLESTIRME_ID = stokNow.DEPO_ESLESTIRME_ID;  
            currentStokH.SORUMLU_ID = stokNow.SORUMLU_ID;
            currentStokH.HAREKET_TIP = stokNow.HAREKET_TIP;
            currentStokH.ACIKLAMA = stokNow.ACIKLAMA;
            currentStokH.HAREKET_MIKTAR = stokNow.HAREKET_MIKTAR;
            currentStokH.HAREKET_TARIHI = stokNow.HAREKET_TARIHI;

            if (string.IsNullOrEmpty(stokNow.STOK_ID.ToString()) || string.IsNullOrEmpty(stokNow.DEPO_ESLESTIRME_ID.ToString()) || string.IsNullOrEmpty(stokNow.SORUMLU_ID.ToString()) || string.IsNullOrEmpty(stokNow.HAREKET_TIP.ToString())
              || string.IsNullOrEmpty(stokNow.HAREKET_MIKTAR.ToString()) || string.IsNullOrEmpty(stokNow.HAREKET_TARIHI.ToString()) || string.IsNullOrEmpty(stokNow.ACIKLAMA.ToString()))
            {
                ViewBag.ErrorMessage = "Stok Hareket Bilgileri Boş Bırakılamaz";
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
            return RedirectToAction("StokHareketiSayfasi");
        }

        public ActionResult ExportToStokHareketExcel()
        {
            // Veritabanından kullanıcı listesini al
            List<STOK_HAREKET> StokHareketList = db.STOK_HAREKET.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("StokHareketList");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Hareket ID";
            workSheet.Cells[1, 2].Value = "Stok ID";
            workSheet.Cells[1, 3].Value = "Sorumlu ID";
            workSheet.Cells[1, 4].Value = "Hareket TIP";
            workSheet.Cells[1, 4].Value = "Açıklama";
            workSheet.Cells[1, 4].Value = "Hareket Miktarı";
            workSheet.Cells[1, 4].Value = "Hareket Tarihi";
            workSheet.Cells[1, 5].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 6].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 7].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 8].Value = "Güncelleme Tarihi";


            // Veri satırlarını ekle
            int row = 2;
            foreach (var stokH in StokHareketList)
            {
                workSheet.Cells[row, 1].Value = stokH.HAREKET_ID;
                workSheet.Cells[row, 2].Value = stokH.STOK_ID;
                workSheet.Cells[row, 3].Value = stokH.DEPO_ESLESTIRME_ID;
                workSheet.Cells[row, 4].Value = stokH.SORUMLU_ID;
                workSheet.Cells[row, 4].Value = stokH.HAREKET_TIP;
                workSheet.Cells[row, 4].Value = stokH.ACIKLAMA;
                workSheet.Cells[row, 4].Value = stokH.HAREKET_MIKTAR;
                workSheet.Cells[row, 4].Value = stokH.HAREKET_TARIHI;
                workSheet.Cells[row, 5].Value = stokH.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 6].Value = stokH.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 7].Value = stokH.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 8].Value = stokH.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"StokHareket_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public ActionResult KullaniciEkleDepo(KULLANICI user)
        {
            if (string.IsNullOrEmpty(user.KUL_USERNAME) || string.IsNullOrEmpty(user.KUL_SIFRE) || string.IsNullOrEmpty(user.KUL_AD) || string.IsNullOrEmpty(user.KUL_SOYAD)
                || string.IsNullOrEmpty(user.STATU.ToString()))
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
                    var newUser = new KULLANICI { GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]), GUNCELLEME_TARIHI = DateTime.Now, OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]), OLUSTURMA_TARIHI = DateTime.Now, KUL_USERNAME = user.KUL_USERNAME, KUL_SIFRE = user.KUL_SIFRE, KUL_AD = user.KUL_AD, KUL_SOYAD = user.KUL_SOYAD, KUL_TIP = 2, STATU = user.STATU };

                    db.KULLANICI.Add(newUser);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("DepoSayfasi");
        }

        public ActionResult KullanıcıSil(int user)
        {
            var us = db.KULLANICI.Find(user);
            db.KULLANICI.Remove(us);
            db.SaveChanges();
            return RedirectToAction("DepoSayfasi");
        }

        [HttpGet]
        public ActionResult DepoSayfasi()
        {
            var userList = db.KULLANICI.Where(x => x.KUL_TIP.ToString() == "2").ToList();
            TempData["Kullanicilar"] = userList;
            TempData["SuccessMessage"] = "Kullanıcı Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Kullanıcı Listesi yüklenirken bir sorun oluştu!";

            return View();
        }
        public ActionResult ExportToExcel()
        {
            // Veritabanından kullanıcı listesini al
            List<KULLANICI> kullaniciList = db.KULLANICI.Where(x => x.KUL_TIP.ToString() == "2").ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Kullanicilar");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Kullanıcı Username";
            workSheet.Cells[1, 2].Value = "Kullanıcı Adı";
            workSheet.Cells[1, 3].Value = "Kullanıcı Soyadı";
            workSheet.Cells[1, 4].Value = "Statu";
            workSheet.Cells[1, 5].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 6].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 7].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 8].Value = "Güncelleme Tarihi";


            // Veri satırlarını ekle
            int row = 2;
            foreach (var kullanici in kullaniciList)
            {
                workSheet.Cells[row, 1].Value = kullanici.KUL_USERNAME;
                workSheet.Cells[row, 2].Value = kullanici.KUL_AD;
                workSheet.Cells[row, 3].Value = kullanici.KUL_SOYAD;
                workSheet.Cells[row, 4].Value = kullanici.STATU;
                workSheet.Cells[row, 5].Value = kullanici.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 6].Value = kullanici.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 7].Value = kullanici.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 8].Value = kullanici.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"DepoKullaniciList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        [HttpGet]
        public ActionResult DepoKullanıcıGüncelle(int id)
        {
            var currentKullanici = db.KULLANICI.Find(id);

            return View(currentKullanici);
        }

        [HttpPost]
        public ActionResult DepoKullanıcıGüncelle(int id, KULLANICI userNow)
        {


            var currentUser = db.KULLANICI.Find(id);
            currentUser.KUL_USERNAME = userNow.KUL_USERNAME;
            currentUser.KUL_AD = userNow.KUL_AD;
            currentUser.KUL_SOYAD = userNow.KUL_SOYAD;
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
            return RedirectToAction("DepoSayfasi");
        }
        public ActionResult StokDurumSayfası()
        {
            var stokDurum = db.STOK_DURUM.ToList();
            TempData["stokDurum"] = stokDurum;
            TempData["SuccessMessage"] = "Stok Hareket Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Stok Hareket Listesi yüklenirken bir sorun oluştu!";

            return View(stokDurum);
        }

        [HttpGet]
        public ActionResult StokDurumEkle()
        {
            var stoklar = db.STOK.ToList();
            ViewBag.stoklar = new SelectList(stoklar, "STOK_ID", "STOK_AD");

            var eslesmeler = db.DEPO_ESLESTIRME.ToList();
            ViewBag.eslesmeler = new SelectList(eslesmeler, "DEPO_ESLESTIRME_ID", "DEPO_ESLESTIRME_ID");

            return View();
        }

        [HttpPost]
        public ActionResult StokDurumEkleF(STOK_DURUM durum, int STOK_ID, int DEPO_ESLESTIRME_ID)
        {
            if (string.IsNullOrEmpty(durum.STOK_ID.ToString()) || string.IsNullOrEmpty(durum.DEPO_ESLESTIRME_ID.ToString()) || string.IsNullOrEmpty(durum.DURUM_MIKTAR.ToString())) 
            { 

                TempData["ErrorMessage"] = "Stok Durum Bilgileri Boş Bırakılamaz";
                return RedirectToAction("StokDurumEkle");
            }
            else
            {

                var newDurum = new STOK_DURUM
                {
                    GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]),
                    GUNCELLEME_TARIHI = DateTime.Now,
                    OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]),
                    OLUSTURMA_TARIHI = DateTime.Now,
                    STOK_ID = STOK_ID,
                    DEPO_ESLESTIRME_ID = DEPO_ESLESTIRME_ID,
                    DURUM_MIKTAR = durum.DURUM_MIKTAR,
                    
                };

                db.STOK_DURUM.Add(newDurum);
                db.SaveChanges();
            }

            return RedirectToAction("StokDurumSayfası");
        }

        public ActionResult StokDurumSil(int stock)
        {
            var st = db.STOK_DURUM.Find(stock);
            db.STOK_DURUM.Remove(st);
            db.SaveChanges();
            return RedirectToAction("StokDurumSayfası");
        }


        public ActionResult ExportToStokDurumExcel()
        {
            // Veritabanından kullanıcı listesini al
            List<STOK_DURUM> StokDurumList = db.STOK_DURUM.ToList();

            // Excel paketi oluştur
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("StokDurumList");

            // Başlık satırı ekle
            workSheet.Cells[1, 1].Value = "Durum ID";
            workSheet.Cells[1, 2].Value = "Stok ID";
            workSheet.Cells[1, 3].Value = "Depo Eşleştirme ID";
            workSheet.Cells[1, 4].Value = "Durum Miktarı";
            workSheet.Cells[1, 5].Value = "Oluşturan Kullanıcı";
            workSheet.Cells[1, 6].Value = "Oluşturma Tarihi";
            workSheet.Cells[1, 7].Value = "Güncelleyen Kullanıcı";
            workSheet.Cells[1, 8].Value = "Güncelleme Tarihi";


            // Veri satırlarını ekle
            int row = 2;
            foreach (var stokH in StokDurumList)
            {
                workSheet.Cells[row, 1].Value = stokH.DURUM_ID;
                workSheet.Cells[row, 2].Value = stokH.STOK_ID;
                workSheet.Cells[row, 3].Value = stokH.DEPO_ESLESTIRME_ID;
                workSheet.Cells[row, 4].Value = stokH.DURUM_MIKTAR;
                workSheet.Cells[row, 5].Value = stokH.OLUSTURAN_KULLANICI;
                workSheet.Cells[row, 6].Value = stokH.OLUSTURMA_TARIHI;
                workSheet.Cells[row, 7].Value = stokH.GUNCELLEYEN_KULLANICI;
                workSheet.Cells[row, 8].Value = stokH.GUNCELLEME_TARIHI;
                row++;

            }

            // Excel dosyasını MemoryStream'e yaz
            using (var memoryStream = new MemoryStream())
            {
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Excel dosyasını indirme işlemi
                string excelName = $"StokDurum_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}