﻿using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Drawing;
using ShapeCrawler.Exceptions;
using A = DocumentFormat.OpenXml.Drawing;

namespace ShapeCrawler.AutoShapes
{
    /// <summary>
    ///     Represents a paragraph bullet.
    /// </summary>
    public class Bullet
    {
        private readonly A.ParagraphProperties aParagraphProperties;
        private readonly Lazy<string> character;
        private readonly Lazy<string> colorHex;
        private readonly Lazy<string> fontName;
        private readonly Lazy<int> size;
        private readonly Lazy<BulletType> type;

        internal Bullet(A.ParagraphProperties aParagraphProperties)
        {
            this.aParagraphProperties = aParagraphProperties;
            this.type = new Lazy<BulletType>(this.ParseType);
            this.colorHex = new Lazy<string>(this.ParseColorHex);
            this.character = new Lazy<string>(this.ParseChar);
            this.fontName = new Lazy<string>(this.ParseFontName);
            this.size = new Lazy<int>(this.ParseSize);
        }

        #region Public Properties

        /// <summary>
        ///     Gets RGB color in HEX format.
        /// </summary>
        public string ColorHex => this.colorHex.Value;

        /// <summary>
        ///     Gets bullet character.
        /// </summary>
        public string Character
        {
            get { return this.character.Value; }
            set
            {
                if (this.aParagraphProperties == null)
                    return;

                if (Type != BulletType.Character)
                    return;

                A.CharacterBullet? aCharBullet = this.aParagraphProperties.GetFirstChild<A.CharacterBullet>();
                if (aCharBullet == null)
                {
                    aCharBullet = new CharacterBullet();
                    this.aParagraphProperties.AddChild(aCharBullet);
                }

                aCharBullet.Char = value;

            }
        }

        /// <summary>
        ///     Gets bullet font name.
        /// </summary>
        public string FontName
        {
            get { return this.fontName.Value; }
            set
            {
                if (this.aParagraphProperties == null)
                    return;

                if (Type == BulletType.None)
                    return;

                A.BulletFont? aBulletFont = this.aParagraphProperties.GetFirstChild<A.BulletFont>();
                if (aBulletFont == null)
                {
                    aBulletFont = new BulletFont();
                    this.aParagraphProperties.AddChild(aBulletFont);
                }
                aBulletFont.Typeface = value;
            }
        }

        /// <summary>
        ///     Gets bullet size.
        /// </summary>
        public int Size
        {
            get { return this.size.Value; }
            set
            {
                if (this.aParagraphProperties == null)
                    return;

                A.BulletSizePercentage? aBulletSizePercent = this.aParagraphProperties.GetFirstChild<A.BulletSizePercentage>();
                if (aBulletSizePercent == null)
                {
                    aBulletSizePercent = new A.BulletSizePercentage();
                    this.aParagraphProperties.AddChild(aBulletSizePercent);
                }

                aBulletSizePercent.Val = value * 1000;
            }
        }

        /// <summary>
        ///     Gets bullet type.
        /// </summary>
        public BulletType Type
        {
            get
            {
                return this.type.Value;
            }
            set
            {
                if (this.aParagraphProperties == null)
                    return;

                A.AutoNumberedBullet? aAutoNumeredBullet = this.aParagraphProperties.GetFirstChild<A.AutoNumberedBullet>();
                this.aParagraphProperties.RemoveChild(aAutoNumeredBullet);

                A.PictureBullet? aPictureBullet = this.aParagraphProperties.GetFirstChild<A.PictureBullet>();
                this.aParagraphProperties.RemoveChild(aPictureBullet);

                A.CharacterBullet? aCharBullet = this.aParagraphProperties.GetFirstChild<A.CharacterBullet>();
                this.aParagraphProperties.RemoveChild(aCharBullet);


                if (value == BulletType.Numbered)
                {
                    var child = new AutoNumberedBullet();
                    // replace at property
                    child.Type = DocumentFormat.OpenXml.Drawing.TextAutoNumberSchemeValues.ArabicPeriod;
                    this.aParagraphProperties.AddChild(child);
                }

                if (value == BulletType.Picture)
                {
                    var child = new PictureBullet();
                    this.aParagraphProperties.AddChild(child);
                }

                if (value == BulletType.Character)
                {
                    var child = new CharacterBullet();
                    this.aParagraphProperties.AddChild(child);
                }
            }

        }

        #endregion Public Properties

        private BulletType ParseType()
        {
            if (this.aParagraphProperties == null)
            {
                return BulletType.None;
            }

            A.AutoNumberedBullet? aAutoNumeredBullet = this.aParagraphProperties.GetFirstChild<A.AutoNumberedBullet>();
            if (aAutoNumeredBullet != null)
            {
                return BulletType.Numbered;
            }

            A.PictureBullet? aPictureBullet = this.aParagraphProperties.GetFirstChild<A.PictureBullet>();
            if (aPictureBullet != null)
            {
                return BulletType.Picture;
            }

            A.CharacterBullet? aCharBullet = this.aParagraphProperties.GetFirstChild<A.CharacterBullet>();
            if (aCharBullet != null)
            {
                return BulletType.Character;
            }

            return BulletType.None;
        }

        private string? ParseColorHex()
        {
            if (this.Type == BulletType.None)
            {
                return null;
            }

            IEnumerable<A.RgbColorModelHex> aRgbClrModelHexCollection = this.aParagraphProperties.Descendants<A.RgbColorModelHex>();
            if (aRgbClrModelHexCollection.Any())
            {
                return aRgbClrModelHexCollection.Single().Val;
            }

            return null;
        }

        private string? ParseChar()
        {
            if (this.Type == BulletType.None)
            {
                return null;
            }

            A.CharacterBullet? aCharBullet = this.aParagraphProperties.GetFirstChild<A.CharacterBullet>();
            if (aCharBullet == null)
            {
                throw new RuntimeDefinedPropertyException($"This is not {nameof(BulletType.Character)} type bullet.");
            }

            return aCharBullet.Char?.Value;
        }

        private string? ParseFontName()
        {
            if (this.Type == BulletType.None)
            {
                return null;
            }

            A.BulletFont? aBulletFont = this.aParagraphProperties.GetFirstChild<A.BulletFont>();
            return aBulletFont?.Typeface?.Value;
        }

        private int ParseSize()
        {
            if (this.Type == BulletType.None)
            {
                return 0;
            }

            A.BulletSizePercentage? aBulletSizePercent = this.aParagraphProperties.GetFirstChild<A.BulletSizePercentage>();
            int basicPoints = aBulletSizePercent?.Val?.Value ?? 100000;

            return basicPoints / 1000;
        }
    }
}