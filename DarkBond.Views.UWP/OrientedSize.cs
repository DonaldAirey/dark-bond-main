// <copyright file="OrientedSize.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using System.Runtime.InteropServices;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// The OrientedSize structure is used to abstract the growth direction from the layout algorithms of WrapPanel.  When the growth direction is
    /// oriented horizontally (ex: the next element is arranged on the side of the previous element), then the Width grows directly with the
    /// placement of elements and Height grows indirectly with the size of the largest element in the row.  When the orientation is reversed, so is
    /// the directional growth with respect to Width and Height.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [StructLayout(LayoutKind.Sequential)]
    internal struct OrientedSize
    {
        /// <summary>
        /// The size dimension that grows directly with layout placement.
        /// </summary>
        private double directField;

        /// <summary>
        /// The size dimension that grows indirectly with the maximum value of the layout row or column.
        /// </summary>
        private double indirectField;

        /// <summary>
        /// The orientation of the structure.
        /// </summary>
        private Orientation orientationField;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedSize"/> struct.
        /// </summary>
        /// <param name="orientation">Orientation of the structure.</param>
        public OrientedSize(Orientation orientation)
            : this(orientation, 0.0, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrientedSize"/> struct.
        /// </summary>
        /// <param name="orientation">Orientation of the structure.</param>
        /// <param name="width">Orientation-free width of the structure.</param>
        /// <param name="height">Orientation-free height of the structure.</param>
        public OrientedSize(Orientation orientation, double width, double height)
        {
            // Initialize the object.
            this.orientationField = orientation;

            // All fields must be initialized before we access the this pointer
            this.directField = 0.0;
            this.indirectField = 0.0;

            // Set the values based on the orientation.
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the orientation of the structure.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.orientationField;
            }
        }

        /// <summary>
        /// Gets or sets the size dimension that grows directly with layout placement.
        /// </summary>
        public double Direct
        {
            get
            {
                return this.directField;
            }

            set
            {
                this.directField = value;
            }
        }

        /// <summary>
        /// Gets or sets the size dimension that grows indirectly with the maximum value of the layout row or column.
        /// </summary>
        public double Indirect
        {
            get
            {
                return this.indirectField;
            }

            set
            {
                this.indirectField = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the size.
        /// </summary>
        public double Width
        {
            get
            {
                return (this.Orientation == Orientation.Horizontal) ? this.Direct : this.Indirect;
            }

            set
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.Direct = value;
                }
                else
                {
                    this.Indirect = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the size.
        /// </summary>
        public double Height
        {
            get
            {
                return (this.Orientation != Orientation.Horizontal) ? this.Direct : this.Indirect;
            }

            set
            {
                if (this.Orientation != Orientation.Horizontal)
                {
                    this.Direct = value;
                }
                else
                {
                    this.Indirect = value;
                }
            }
        }
    }
}