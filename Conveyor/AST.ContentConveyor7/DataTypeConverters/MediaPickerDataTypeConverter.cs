﻿using System.Configuration;
using System.Linq;
using Umbraco.Core.Configuration;

namespace AST.ContentConveyor7.DataTypeConverters
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using AST.ContentConveyor7;
    using AST.ContentConveyor7.Enums;
    using AST.ContentConveyor7.Utilities;

    using Umbraco.Core.Models;

    public class MediaPickerDataTypeConverter : BaseContentManagement, IDataTypeConverter
    {
        public void Export(Property property, XElement propertyTag, Dictionary<int, ObjectTypes> dependantNodes)
        {
            if (property.Value != null && !string.IsNullOrWhiteSpace(property.Value.ToString()))
            {
                var id = int.Parse(property.Value.ToString());
                var media = Services.MediaService.GetById(id);

                var uploadFieldAlias = GetUploadFieldAlias();

                // TODO for v6
                if (media != null && FileHelpers.FileExists(media.GetValue(uploadFieldAlias).ToString())) 
                {
                    propertyTag.Value = media.Key.ToString();

                    if (!dependantNodes.ContainsKey(media.Id))
                    {
                        dependantNodes.Add(media.Id, ObjectTypes.Media);
                    }
                }
            }
        }

        public string Import(XElement propertyTag)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(propertyTag.Value))
            {
                var guid = new Guid(propertyTag.Value);
                var id = Services.MediaService.GetById(guid).Id;
                result = id.ToString();
            }

            return result;
        }

        private string GetUploadFieldAlias()
        {
            var uploadFields = UmbracoConfig.For.UmbracoSettings().Content.ImageAutoFillProperties.ToList();
            if (uploadFields == null || !uploadFields.Any(f => string.IsNullOrEmpty(f.Alias)))
            {
                throw new ConfigurationErrorsException("Expected /content/imaging/autoFillImageProperties/uploadField alias attribute");
            }

            return uploadFields.FirstOrDefault(f => !string.IsNullOrEmpty(f.Alias)).Alias;
        }
    }
}