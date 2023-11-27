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
        public BaseConfig activeConfig;
        public BaseConfig tempConfig;

        public SEChatGPTConfigScreen() : base(size: new Vector2(1, 0.9f))
        {
            //Grab config
            activeConfig = ConfigService.ActiveConfig;
            tempConfig = ConfigService.ActiveConfig.Clone();
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

            //Enable
            MyGuiControlLabel lblEnable = new MyGuiControlLabel(text: "SEChatGPT");
            PositionX(lblEnable, title, -GuiSpacing * 2, true);
            PositionY(lblEnable, title, GuiSpacing * 3);
            Controls.Add(lblEnable);

            MyGuiControlOnOffSwitch cbEnable = new MyGuiControlOnOffSwitch(activeConfig.Enabled);
            cbEnable.Name = "cbEnable";
            PositionY(cbEnable, lblEnable, GuiSpacing * 3, true);
            PositionX(cbEnable, lblEnable, GuiSpacing * 2);
            Controls.Add(cbEnable);


            //API Keys
            MyGuiControlLabel lblGPTAPIKey = new MyGuiControlLabel(text: "GPT API Key");
            PositionY(lblGPTAPIKey, cbEnable, GuiSpacing * 3.5f, true);
            PositionX(lblGPTAPIKey, cbEnable, -GuiSpacing * 22f);
            Controls.Add(lblGPTAPIKey);

            MyGuiControlTextbox tbGPTAPIKey = new MyGuiControlTextbox(null, activeConfig.GPTAPIKey);
            tbGPTAPIKey.Name = "tbGPTAPIKey";
            PositionY(tbGPTAPIKey, lblGPTAPIKey, GuiSpacing * 2, true);
            PositionX(tbGPTAPIKey, lblGPTAPIKey, GuiSpacing * 9f);
            Controls.Add(tbGPTAPIKey);

            MyGuiControlLabel lblElevanLabsAPIKey = new MyGuiControlLabel(text: "ElevanLabs API Key");
            PositionX(lblElevanLabsAPIKey, lblGPTAPIKey, GuiSpacing * 20, true);
            Controls.Add(lblElevanLabsAPIKey);

            MyGuiControlTextbox tbElevanLabsAPIKey = new MyGuiControlTextbox(null, activeConfig.ElevanLabsAPIKey);
            tbElevanLabsAPIKey.Name = "tbElevanLabsAPIKey";
            PositionY(tbElevanLabsAPIKey, lblElevanLabsAPIKey, GuiSpacing * 2, true);
            PositionX(tbElevanLabsAPIKey, lblElevanLabsAPIKey, GuiSpacing * 9f);
            Controls.Add(tbElevanLabsAPIKey);


            //GPT Settings
            MyGuiControlLabel lblGPTModel = new MyGuiControlLabel(text: "GPT Model");
            PositionY(lblGPTModel, tbGPTAPIKey, GuiSpacing * 3.5f, true);
            PositionX(lblGPTModel, tbGPTAPIKey, -GuiSpacing * 9);
            Controls.Add(lblGPTModel);

            MyGuiControlCombobox cbGPTModel = new MyGuiControlCombobox();
            cbGPTModel.Name = "cbGPTModel";
            cbGPTModel.SetMaxWidth(GuiSpacing * 8);
            var modelNames = Enum.GetNames(typeof(GPTModel)).ToList();
            for (int i = 0; i < modelNames.Count; i++)
            {
                cbGPTModel.AddItem(i, modelNames[i]);
                if (activeConfig.GPTModel == (GPTModel)i)
                {
                    cbGPTModel.SelectItemByIndex(i);
                }
            }
            PositionY(cbGPTModel, lblGPTModel, GuiSpacing * 2.5f, true);
            PositionX(cbGPTModel, lblGPTModel, GuiSpacing * 4);
            Controls.Add(cbGPTModel);

            MyGuiControlLabel lblInputType = new MyGuiControlLabel(text: "Input Type");
            PositionX(lblInputType, lblGPTModel, GuiSpacing * 10, true);
            Controls.Add(lblInputType);

            MyGuiControlCombobox cbInputType = new MyGuiControlCombobox();
            cbInputType.Name = "cbInputType";
            cbInputType.SetMaxWidth(GuiSpacing * 8);
            var inputNames = Enum.GetNames(typeof(InputType)).ToList();
            for (int i = 0; i < inputNames.Count; i++)
            {
                cbInputType.AddItem(i, inputNames[i]);
                if (activeConfig.InputType == (InputType)i)
                {
                    cbInputType.SelectItemByIndex(i);
                }
            }
            PositionY(cbInputType, lblInputType, GuiSpacing * 2.5f, true);
            PositionX(cbInputType, lblInputType, GuiSpacing * 4);
            Controls.Add(cbInputType);

            MyGuiControlLabel lblOutputType = new MyGuiControlLabel(text: "Output Type");
            PositionX(lblOutputType, lblInputType, GuiSpacing * 10, true);
            Controls.Add(lblOutputType);

            MyGuiControlCombobox cbOutputType = new MyGuiControlCombobox();
            cbOutputType.Name = "cbOutputType";
            cbOutputType.SetMaxWidth(GuiSpacing * 8);
            var outputNames = Enum.GetNames(typeof(OutputType)).ToList();
            for (int i = 0; i < outputNames.Count; i++)
            {
                cbOutputType.AddItem(i, outputNames[i]);
                if (activeConfig.OutputType == (OutputType)i)
                {
                    cbOutputType.SelectItemByIndex(i);
                }
            }
            PositionY(cbOutputType, lblOutputType, GuiSpacing * 2.5f, true);
            PositionX(cbOutputType, lblOutputType, GuiSpacing * 4);
            Controls.Add(cbOutputType);

            //Behaviour
            MyGuiControlLabel lblBehaviour = new MyGuiControlLabel(text: "GPT Behaviour");
            PositionY(lblBehaviour, cbGPTModel, GuiSpacing * 3.5f, true);
            PositionX(lblBehaviour, cbGPTModel, -GuiSpacing * 4);
            Controls.Add(lblBehaviour);

            MyGuiControlMultilineEditableText tbBehaviour = new MyGuiControlMultilineEditableText(drawScrollbarV: true);
            tbBehaviour.Name = "tbBehaviour";
            tbBehaviour.Size = new Vector2(GuiSpacing * 44, GuiSpacing * 20);
            tbBehaviour.TextWrap = true;
            tbBehaviour.TextBoxAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP;
            tbBehaviour.TextPadding = new MyGuiBorderThickness(0.01f);
            tbBehaviour.Text = new StringBuilder(activeConfig.GPTBehaviour);
            PositionY(tbBehaviour, lblBehaviour, GuiSpacing * 11.5f, true);
            PositionX(tbBehaviour, lblBehaviour, GuiSpacing * 22f);
            Controls.Add(tbBehaviour);
        }

        private void OnApplyClick(MyGuiControlButton btn)
        {
            foreach (var control in Controls)
            {
                switch (control.Name)
                {
                    case "cbEnable":
                        tempConfig.Enabled = ((MyGuiControlOnOffSwitch)control).Enabled;
                        break;
                    case "tbGPTAPIKey":
                        tempConfig.GPTAPIKey = ((MyGuiControlTextbox)control).Text;
                        break;
                    case "tbElevanLabsAPIKey":
                        tempConfig.ElevanLabsAPIKey = ((MyGuiControlTextbox)control).Text;
                        break;
                    case "cbGPTModel":
                        tempConfig.GPTModel = (GPTModel)((MyGuiControlCombobox)control).GetSelectedIndex();
                        break;
                    case "cbInputType":
                        tempConfig.InputType = (InputType)((MyGuiControlCombobox)control).GetSelectedIndex();
                        break;
                    case "cbOutputType":
                        tempConfig.OutputType = (OutputType)((MyGuiControlCombobox)control).GetSelectedIndex();
                        break;
                    case "tbBehaviour":
                        tempConfig.GPTBehaviour = ((MyGuiControlMultilineEditableText)control).Text.ToString();
                        break;
                }
            }

            ConfigService.Save(tempConfig);
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
