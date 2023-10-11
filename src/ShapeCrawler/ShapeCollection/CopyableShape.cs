﻿using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.Wrappers;
using P = DocumentFormat.OpenXml.Presentation;
using Shape = ShapeCrawler.ShapeCollection.Shape;

namespace ShapeCrawler.ShapeCollection;

internal abstract class CopyableShape : Shape
{
    internal CopyableShape(TypedOpenXmlPart sdkTypedOpenXmlPart, OpenXmlElement openXmlElement)
        : base(sdkTypedOpenXmlPart, openXmlElement)
    {
    }

    internal virtual void CopyTo(
        int id,
        P.ShapeTree pShapeTree,
        IEnumerable<string> existingShapeNames)
    {
        new PShapeTreeWrap(pShapeTree).Add(this.pShapeTreeElement);
    }
}