﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoFilter.Win10
{
    public class BitmapCache
    {
        private static List<BitmapCacheItem> _cache;

        private static BitmapCache _instance;

        private BitmapCache()
        {
            _cache = new List<BitmapCacheItem>();
        }

        public static BitmapCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BitmapCache();
                }
                return _instance;
            }
        }

        public async Task<WriteableBitmap> RetrieveAndPutBitmapAsync(StorageFile file)
        {
            IRandomAccessStream contents = await file.OpenAsync(FileAccessMode.Read);
            //WriteableBitmap bitmap = new WriteableBitmap(100, 100);
            //bitmap.SetSource(contents);
            WriteableBitmap bitmap = await BitmapFactory.New(1, 1).FromStream(contents);

            _cache.Add(new BitmapCacheItem() { StorageFile = file, Bitmap = bitmap });
            Debug.WriteLine("Caching bitmap for file: " + file.Path + "::" + file.Name);
            return bitmap;
        }

        public async Task<WriteableBitmap> GetBitmapAsync(StorageFile file)
        {
            Debug.WriteLine("Check cache for file: " + file.Path + "::" + file.Name);
            WriteableBitmap bitmap = _cache.Find(
                (x) => x.StorageFile.Name == file.Name && x.StorageFile.Path == file.Path
                ).Bitmap;

            if (bitmap == null)
                bitmap = await RetrieveAndPutBitmapAsync(file);

            return bitmap;

        }

        private struct BitmapCacheItem
        {
            public StorageFile StorageFile { get; set; }
            public WriteableBitmap Bitmap { get; set; }
        }
    }
}
