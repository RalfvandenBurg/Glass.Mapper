/*
   Copyright 2012 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/ 
//-CRE-


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Diagnostics;
using Sitecore.Resources;
using Sitecore.Configuration;
using Sitecore;
using Sitecore.IO;
using Sitecore.Web;

namespace Glass.Mapper.Sc.DataMappers
{
    /// <summary>
    /// Class SitecoreFieldImageMapper
    /// </summary>
    public class SitecoreFieldImageMapper : AbstractSitecoreFieldMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreFieldImageMapper"/> class.
        /// </summary>
        public SitecoreFieldImageMapper() : base(typeof (Image))
        {
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="config">The config.</param>
        /// <param name="context">The context.</param>
        /// <returns>System.Object.</returns>
        public override object GetField(Field field, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            Image img = new Image();
            ImageField scImg = new ImageField(field);

            MapToImage(img, scImg);

            return img;
        }

        public static void MapToImage(Image img, ImageField field)
        {
            int height = 0;
            int.TryParse(field.Height, out height);
            int width = 0;
            int.TryParse(field.Width, out width);
            int hSpace = 0;
            int.TryParse(field.HSpace, out hSpace);
            int vSpace = 0;
            int.TryParse(field.VSpace, out vSpace);

            img.Alt = field.Alt;
            img.Border = field.Border;
            img.Class = field.Class;
            img.Height = height;
            img.HSpace = hSpace;
            img.MediaId = field.MediaID.Guid;
            if (field.MediaItem != null)
            {
                img.Src = GetMediaUrlWithoutContentCheck(field.MediaItem, MediaUrlOptions.Empty);
                var fieldTitle = field.MediaItem.Fields["Title"];
                if (fieldTitle != null)
                    img.Title = fieldTitle.Value;
            }
            img.VSpace = vSpace;
            img.Width = width;
        }
        
        public static void MapToImage(Image img, MediaItem imageItem)
        {
           /* int height = 0;
            int.TryParse(imageItem..Height, out height);
            int width = 0;
            int.TryParse(imageItem.Width, out width);
            int hSpace = 0;
            int.TryParse(imageItem.HSpace, out hSpace);
            int vSpace = 0;
            int.TryParse(imageItem.VSpace, out vSpace);*/

            img.Alt = imageItem.Alt;
            img.Title = imageItem.Title;
           // img.Border = imageItem.Border;
           // img.Class = imageItem.Class;
           // img.Height = height;
           // img.HSpace = hSpace;
            img.MediaId = imageItem.ID.Guid;
            img.Src = GetMediaUrlWithoutContentCheck(imageItem, MediaUrlOptions.Empty);
           // img.VSpace = vSpace;
           // img.Width = width;
        }

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="config">The config.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="Glass.Mapper.MapperException">No item with ID {0}. Can not update Media Item field.Formatted(newId)</exception>
        public override void SetField(Field field, object value, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            Image img = value as Image;
            var item = field.Item;

            if (field == null) return;

            ImageField scImg = new ImageField(field);

            MapToField(scImg, img, item);
        }

        public static void MapToField(ImageField field, Image image, Item item)
        {
            if (image == null)
            {
                field.Clear();
                return;
            }

            if (field.MediaID.Guid != image.MediaId)
            {
                //this only handles empty guids, but do we need to remove the link before adding a new one?
                if (image.MediaId == Guid.Empty)
                {
                    ItemLink link = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, field.MediaItem.Database.Name, field.MediaID, field.MediaItem.Paths.Path);
                    field.RemoveLink(link);
                }
                else
                {
                    ID newId = new ID(image.MediaId);
                    Item target = item.Database.GetItem(newId);
                    if (target != null)
                    {
                        field.MediaID = newId;
                        ItemLink link = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, target.Database.Name, target.ID, target.Paths.FullPath);
                        field.UpdateLink(link);
                        
                    }
                    else throw new MapperException("No item with ID {0}. Can not update Media Item field".Formatted(newId));
                }
            }

            if(image.Height > 0)
                field.Height = image.Height.ToString();
            if(image.Width > 0)
                field.Width = image.Width.ToString();
            if(image.HSpace > 0)
                field.HSpace = image.HSpace.ToString();
            if(image.VSpace > 0)
                field.VSpace = image.VSpace.ToString();
            
            if(field.Alt.IsNotNullOrEmpty() || image.Alt.IsNotNullOrEmpty())
                field.Alt = image.Alt ?? string.Empty;
            if (field.Border.IsNotNullOrEmpty() || image.Border.IsNotNullOrEmpty())
                field.Border = image.Border ?? string.Empty;
            if (field.Class.IsNotNullOrEmpty() || image.Class.IsNotNullOrEmpty())
                field.Class = image.Class ?? string.Empty;
        }
        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="config">The config.</param>
        /// <param name="context">The context.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string SetFieldValue(object value, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="config">The config.</param>
        /// <param name="context">The context.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object GetFieldValue(string fieldValue, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            var imageItem = new MediaItem(context.Service.Database.GetItem(new ID(fieldValue)));
            var image = new Image();
            MapToImage(image, imageItem);
            return image;
        }

        /// <summary>
        /// Gets a media URL.
        /// 
        /// </summary>
        /// <param name="item">The media item.</param><param name="options">The query string.</param>
        /// <returns>
        /// The media URL.
        /// </returns>
        private static string GetMediaUrlWithoutContentCheck(MediaItem item, MediaUrlOptions options)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)options, "options");
            if (item.InnerItem["path"].Length > 0)
            {
                if (!options.LowercaseUrls)
                    return item.InnerItem["path"];
                else
                    return item.InnerItem["path"].ToLowerInvariant();
            }
            else if (options.UseDefaultIcon)
            {
                if (!options.LowercaseUrls)
                    return Themes.MapTheme(Settings.DefaultIcon);
                else
                    return Themes.MapTheme(Settings.DefaultIcon).ToLowerInvariant();
            }
            else
            {
                var mediaProvider = MediaManager.Provider;
                Assert.IsTrue(mediaProvider.Config.MediaPrefixes[0].Length > 0, "media prefixes are not configured properly.");
                string str1 = mediaProvider.MediaLinkPrefix;
                if (options.AbsolutePath)
                    str1 = options.VirtualFolder + str1;
                else if (str1.StartsWith("/", StringComparison.InvariantCulture))
                    str1 = StringUtil.Mid(str1, 1);
                if (options.AlwaysIncludeServerUrl)
                    str1 = FileUtil.MakePath(WebUtil.GetServerUrl(), str1, '/');
                string str2 = StringUtil.EnsurePrefix('.', StringUtil.GetString(options.RequestExtension, item.Extension, "ashx"));
                string str3 = options.ToString();
                if (str3.Length > 0)
                    str2 = str2 + "?" + str3;
                string str4 = "/sitecore/media library/";
                string path = item.InnerItem.Paths.Path;
                string str5 = !options.UseItemPath || !path.StartsWith(str4, StringComparison.OrdinalIgnoreCase) ? item.ID.ToShortID().ToString() : StringUtil.Mid(path, str4.Length);
                string str6 = str1 + str5 + (options.IncludeExtension ? str2 : string.Empty);
                if (!options.LowercaseUrls)
                    return str6;
                else
                    return str6.ToLowerInvariant();
            }
        }
    }
}




