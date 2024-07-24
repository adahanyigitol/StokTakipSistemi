using Microsoft.Ajax.Utilities;
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
    public class RaporKullanicisiController : Controller
    {
        StokTakipDBEntities db = new StokTakipDBEntities();
        // GET: RaporKullanicisi
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

            KULLANICI users = db.KULLANICI.FirstOrDefault(u => u.KUL_USERNAME == user.KUL_USERNAME && u.KUL_SIFRE == user.KUL_SIFRE && u.KUL_TIP == 3);


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
        public ActionResult RaporKulSayfasi()
        {
            var raporKulList = db.KULLANICI.Where(x => x.KUL_TIP.ToString() == "3").ToList();
            TempData["Kullanicilar"] = raporKulList;
            TempData["SuccessMessage"] = "Kullanıcı Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Kullanıcı Listesi yüklenirken bir sorun oluştu!";

            return View();
        }

        [HttpPost]
        public ActionResult RaporKulEkle(KULLANICI user)
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
                    var newUser = new KULLANICI { GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["KulID"]), GUNCELLEME_TARIHI = DateTime.Now, OLUSTURAN_KULLANICI = Convert.ToInt32(Session["KulID"]), OLUSTURMA_TARIHI = DateTime.Now, KUL_USERNAME = user.KUL_USERNAME, KUL_SIFRE = user.KUL_SIFRE, KUL_AD = user.KUL_AD, KUL_SOYAD = user.KUL_SOYAD, KUL_TIP = 3, STATU = user.STATU };

                    db.KULLANICI.Add(newUser);
                    db.SaveChanges();
                }

            }


            return RedirectToAction("RaporKulSayfasi");
        }
        public ActionResult KullanıcıSil(int user)
        {
            var us = db.KULLANICI.Find(user);
            db.KULLANICI.Remove(us);
            db.SaveChanges();
            return RedirectToAction("RaporKulSayfasi");
        }
        public ActionResult ExportToExcelHareket()
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
        [HttpGet]
        public ActionResult RaporKullaniciGuncelle(int id)
        {
            var currentKullanici = db.KULLANICI.Find(id);

            return View(currentKullanici);
        }

        [HttpPost]
        public ActionResult RaporKullaniciGuncelle(int id, KULLANICI userNow)
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
            return RedirectToAction("RaporKulSayfasi");
        }

        [HttpGet]
        public ActionResult RaporSayfasi()
        {
            var raporList = db.STOK_HAREKET.ToList();
            TempData["stokHareketleri"] = raporList;
            TempData["SuccessMessage"] = "Stok Hareketleri Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Stok Hareketleri Kullanıcı Listesi yüklenirken bir sorun oluştu!";

            return View();
        }

        [HttpGet]
        public ActionResult StokDurumSayfasi()
        {
            var raporList = db.STOK_DURUM.ToList();
            TempData["stokDurum"] = raporList;
            TempData["SuccessMessage"] = "Stok Durum Listesi başarıyla yüklendi.";
            TempData["FailMessage"] = "Stok Durum Kullanıcı Listesi yüklenirken bir sorun oluştu!";

            return View();
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