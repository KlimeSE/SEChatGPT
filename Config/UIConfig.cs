using Sandbox;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;
using VRageMath;

namespace SEChatGPT.Config
{
    public abstract class BaseScreen : MyGuiScreenBase
    {
        public const float GuiSpacing = 0.0175f;

        public BaseScreen(Vector2? position = null, Vector2? size = null) :
            base(position ?? new Vector2(0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, size ?? new Vector2(0.5f),
                backgroundTransition: MySandboxGame.Config.UIBkOpacity, guiTransition: MySandboxGame.Config.UIOpacity)
        {
            EnabledBackgroundFade = true;
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            CanHideOthers = true;
            CanBeHidden = true;
            CloseButtonEnabled = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        protected RectangleF GetAreaBetween(MyGuiControlBase top, MyGuiControlBase bottom, float verticalSpacing = GuiSpacing, float horizontalSpacing = GuiSpacing)
        {
            Vector2 halfSize = m_size.Value / 2;

            float topPosY = GetCoordTopLeftFromAligned(top).Y;
            Vector2 topPos = new Vector2(horizontalSpacing - halfSize.X, topPosY + top.Size.Y + verticalSpacing);

            float bottomPosY = GetCoordTopLeftFromAligned(bottom).Y;
            Vector2 bottomPos = new Vector2(halfSize.X - horizontalSpacing, bottomPosY - verticalSpacing);

            Vector2 size = bottomPos - topPos;
            size.X = Math.Abs(size.X);
            size.Y = Math.Abs(size.Y);

            return new RectangleF(topPos, size);
        }

        protected MyLayoutTable GetLayoutTableBetween(MyGuiControlBase top, MyGuiControlBase bottom, float verticalSpacing = GuiSpacing, float horizontalSpacing = GuiSpacing)
        {
            RectangleF rect = GetAreaBetween(top, bottom, verticalSpacing, horizontalSpacing);
            return new MyLayoutTable(this, rect.Position, rect.Size);
        }

        protected void AddBarBelow(MyGuiControlBase control, float barWidth = 0.8f, float spacing = GuiSpacing)
        {
            MyGuiControlSeparatorList bar = new MyGuiControlSeparatorList();
            barWidth *= m_size.Value.X;
            float controlTop = GetCoordTopLeftFromAligned(control).Y;
            bar.AddHorizontal(new Vector2(barWidth * -0.5f, controlTop + spacing + control.Size.Y), barWidth);
            Controls.Add(bar);
        }

        protected void AddBarAbove(MyGuiControlBase control, float barWidth = 0.8f, float spacing = GuiSpacing)
        {
            MyGuiControlSeparatorList bar = new MyGuiControlSeparatorList();
            barWidth *= m_size.Value.X;
            float controlTop = GetCoordTopLeftFromAligned(control).Y;
            bar.AddHorizontal(new Vector2(barWidth * -0.5f, controlTop - spacing), barWidth);
            Controls.Add(bar);
        }

        protected void AdvanceLayout(ref MyLayoutVertical layout, float amount = GuiSpacing)
        {
            layout.Advance(amount * MyGuiConstants.GUI_OPTIMAL_SIZE.Y);
        }

        protected void AdvanceLayout(ref MyLayoutHorizontal layout, float amount = GuiSpacing)
        {
            layout.Advance(amount * MyGuiConstants.GUI_OPTIMAL_SIZE.Y);
        }

        protected Vector2 GetCoordTopLeftFromAligned(MyGuiControlBase control)
        {
            return MyUtils.GetCoordTopLeftFromAligned(control.Position, control.Size, control.OriginAlign);
        }

        /// <summary>
        /// Positions <paramref name="newControl"/> to the right of <paramref name="currentControl"/> with a spacing of <paramref name="spacing"/>.
        /// </summary>
        public void PositionToRight(MyGuiControlBase currentControl, MyGuiControlBase newControl, MyAlignV align = MyAlignV.Center, float spacing = GuiSpacing,
            bool relative = false)
        {
            Vector2 currentTopLeft = GetCoordTopLeftFromAligned(currentControl);
            currentTopLeft.X += currentControl.Size.X + spacing;
            switch (align)
            {
                case MyAlignV.Top:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP;
                    break;
                case MyAlignV.Center:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER;
                    break;
                case MyAlignV.Bottom:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM;
                    break;
                default:
                    return;
            }

            if (relative)
            {
                newControl.Position = new Vector2(currentTopLeft.X, newControl.Position.Y);
            }
            else
            {
                newControl.Position = currentTopLeft;
            }
        }

        public void PositionX(MyGuiControlBase child, MyGuiControlBase parent, float spacing = GuiSpacing, bool relative = false)
        {
            if (relative)
                child.Position = new Vector2(parent.Position.X + spacing, parent.Position.Y);
            else
                child.Position = new Vector2(parent.Position.X + spacing, child.Position.Y);
        }

        public void PositionY(MyGuiControlBase child, MyGuiControlBase parent, float spacing = GuiSpacing, bool relative = false)
        {
            if (relative)
                child.Position = new Vector2(parent.Position.X, parent.Position.Y + spacing);
            else
                child.Position = new Vector2(child.Position.X, parent.Position.Y + spacing);
        }

        /// <summary>
        /// Positions <paramref name="newControl"/> to the left of <paramref name="currentControl"/> with a spacing of <paramref name="spacing"/>.
        /// </summary>
        public void PositionToLeft(MyGuiControlBase currentControl, MyGuiControlBase newControl, MyAlignV align = MyAlignV.Center, float spacing = GuiSpacing,
            bool relative = false)
        {
            Vector2 currentTopLeft = GetCoordTopLeftFromAligned(currentControl);
            currentTopLeft.X -= spacing;
            switch (align)
            {
                case MyAlignV.Top:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP;
                    break;
                case MyAlignV.Center:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER;
                    break;
                case MyAlignV.Bottom:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM;
                    break;
                default:
                    return;
            }

            if (relative)
            {
                newControl.Position = new Vector2(currentTopLeft.X, newControl.Position.Y);
            }
            else
            {
                newControl.Position = currentTopLeft;
            }
        }

        /// <summary>
        /// Positions <paramref name="newControl"/> above <paramref name="currentControl"/> with a spacing of <paramref name="spacing"/>.
        /// </summary>
        public void PositionAbove(MyGuiControlBase currentControl, MyGuiControlBase newControl, MyAlignH align = MyAlignH.Center, float spacing = GuiSpacing,
            bool relative = false)
        {
            Vector2 currentTopLeft = GetCoordTopLeftFromAligned(currentControl);
            currentTopLeft.Y -= spacing;
            switch (align)
            {
                case MyAlignH.Left:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM;
                    break;
                case MyAlignH.Center:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM;
                    break;
                case MyAlignH.Right:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM;
                    break;
                default:
                    return;
            }

            if (relative)
            {
                newControl.Position = new Vector2(newControl.Position.X, currentTopLeft.Y);
            }
            else
            {
                newControl.Position = currentTopLeft;
            }
        }

        /// <summary>
        /// Positions <paramref name="newControl"/> below <paramref name="currentControl"/> with a spacing of <paramref name="spacing"/>.
        /// </summary>
        public void PositionBelow(MyGuiControlBase currentControl, MyGuiControlBase newControl, MyAlignH align = MyAlignH.Center, float spacing = GuiSpacing,
            bool relative = false)
        {
            Vector2 currentTopLeft = GetCoordTopLeftFromAligned(currentControl);
            currentTopLeft.Y += currentControl.Size.Y + spacing;
            switch (align)
            {
                case MyAlignH.Left:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP;
                    break;
                case MyAlignH.Center:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP;
                    break;
                case MyAlignH.Right:
                    newControl.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP;
                    break;
                default:
                    return;
            }

            if (relative)
            {
                newControl.Position = new Vector2(newControl.Position.X, currentTopLeft.Y);
            }
            else
            {
                newControl.Position = currentTopLeft;
            }
        }

        protected void AddImageToButton(MyGuiControlButton button, string iconTexture, float iconSize = 1)
        {
            MyGuiControlImage icon = new MyGuiControlImage(size: button.Size * iconSize, textures: new[] { iconTexture });
            icon.Enabled = button.Enabled;
            icon.HasHighlight = button.HasHighlight;
            button.Elements.Add(icon);
        }

        protected void SetTableHeight(MyGuiControlTable table, float height)
        {
            float numRows = height / table.RowHeight;
            table.VisibleRowsCount = Math.Max((int)numRows - 1, 1);
        }
    }

    public class SEChatGPTConfigScreen : BaseScreen
    {
        public SEChatGPTConfigScreen() : base(size: new Vector2(1, 0.9f))
        {

        }

        public static void Open()
        {
            SEChatGPTConfigScreen wheelScreen = new SEChatGPTConfigScreen();
            MyGuiSandbox.AddScreen(wheelScreen);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            // Top
            MyGuiControlLabel title = AddCaption("SEChatGPT Settings", captionScale: 1);
            AddBarBelow(title);

            // Bottom
            Vector2 bottomMid = new Vector2(0, m_size.Value.Y / 2);
            MyGuiControlButton btnApply = new MyGuiControlButton(position: new Vector2(bottomMid.X - GuiSpacing, bottomMid.Y - GuiSpacing),
                text: new StringBuilder("Apply"), originAlign: VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM, onButtonClick: OnApplyClick);
            MyGuiControlButton btnCancel = new MyGuiControlButton(position: new Vector2(bottomMid.X + GuiSpacing, bottomMid.Y - GuiSpacing),
                text: new StringBuilder("Cancel"), originAlign: VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM, onButtonClick: OnCancelClick);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);
            AddBarAbove(btnApply);

            MyGuiControlLabel lblEnable = new MyGuiControlLabel(text: "SEChatGPT");
            PositionX(lblEnable, title, -GuiSpacing * 2, true);
            PositionY(lblEnable, title, GuiSpacing * 5);
            Controls.Add(lblEnable);

            MyGuiControlOnOffSwitch cbEnable = new MyGuiControlOnOffSwitch(true);
            PositionY(cbEnable, lblEnable, GuiSpacing * 3, true);
            PositionX(cbEnable, lblEnable, GuiSpacing * 2);
            Controls.Add(cbEnable);
            
            MyGuiControlLabel lblGPTAPIKey = new MyGuiControlLabel(text: "GPT API Key");
            PositionY(lblGPTAPIKey, cbEnable, GuiSpacing * 3, true);
            PositionX(lblGPTAPIKey, cbEnable, -GuiSpacing * 1.8f);
            Controls.Add(lblGPTAPIKey);

            MyGuiControlTextbox tbGPTAPIKey = new MyGuiControlTextbox();
            PositionY(tbGPTAPIKey, lblGPTAPIKey, GuiSpacing * 2, true);
            PositionX(tbGPTAPIKey, lblGPTAPIKey, GuiSpacing * 2.5f);
            Controls.Add(tbGPTAPIKey);

            MyGuiControlLabel lblGPTModel = new MyGuiControlLabel(text: "GPT Model");
            PositionY(lblGPTModel, tbGPTAPIKey, GuiSpacing * 3, true);
            PositionX(lblGPTModel, tbGPTAPIKey, -GuiSpacing * 2);
            Controls.Add(lblGPTModel);

            MyGuiControlCombobox cbGPTModel = new MyGuiControlCombobox();
            cbGPTModel.AddItem(0, "GPT-3");
            cbGPTModel.AddItem(1, "GPT-4");
            PositionY(cbGPTModel, lblGPTModel, GuiSpacing * 2.5f, true);
            PositionX(cbGPTModel, lblGPTModel, GuiSpacing * 2);
            Controls.Add(cbGPTModel);

            MyGuiControlLabel lblInputType = new MyGuiControlLabel(text: "Input Type");
            PositionY(lblInputType, cbGPTModel, GuiSpacing * 3, true);
            PositionX(lblInputType, cbGPTModel, -GuiSpacing * 2);
            Controls.Add(lblInputType);

            MyGuiControlCombobox cbInputType = new MyGuiControlCombobox();
            cbInputType.AddItem(0, "Text");
            cbInputType.AddItem(1, "Voice");
            PositionY(cbInputType, lblInputType, GuiSpacing * 2.5f, true);
            PositionX(cbInputType, lblInputType, GuiSpacing * 2);
            Controls.Add(cbInputType);

            MyGuiControlLabel lblOutputType = new MyGuiControlLabel(text: "Output Type");
            PositionY(lblOutputType, cbInputType, GuiSpacing * 3, true);
            PositionX(lblOutputType, cbInputType, -GuiSpacing * 2);
            Controls.Add(lblOutputType);

            MyGuiControlCombobox cbOutputType = new MyGuiControlCombobox();
            cbOutputType.AddItem(0, "Text");
            cbOutputType.AddItem(1, "Voice");
            cbOutputType.AddItem(2, "Advanced Voice");
            PositionY(cbOutputType, lblOutputType, GuiSpacing * 2.5f, true);
            PositionX(cbOutputType, lblOutputType, GuiSpacing * 2);
            Controls.Add(cbOutputType);

        }

        private void OnApplyClick(MyGuiControlButton btn)
        {
            //Apply settings
            CloseScreen();
        }

        private void OnCancelClick(MyGuiControlButton btn)
        {
            CloseScreen();
        }

        public override string GetFriendlyName()
        {
            return typeof(SEChatGPTConfigScreen).FullName;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }
    }

    public class UIConfig : BaseConfig
    {
        public UIConfig()
        {

        }
    }
}
