// <copyright file="FrameButton.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A button that contains a single image surrounded by a circle.
    /// </summary>
    /// <remarks>This control exists primarily to fix a but with the AppBarButton that prevents the width from being changed.</remarks>
    public class FrameButton : AppBarButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameButton"/> class.
        /// </summary>
        public FrameButton()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(FrameButton);
        }
    }
}