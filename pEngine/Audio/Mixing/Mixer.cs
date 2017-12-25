// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System.Collections.Generic;

using ManagedBass.Mix;

using pEngine.Audio.Base;
using pEngine.Audio.DSP;
using pEngine.Framework;
using pEngine.Framework.Binding;

namespace pEngine.Audio.Mixing
{
    /// <summary>
    /// Manages channels and mix all sounds.
    /// </summary>
    public class Mixer : pObject, IMixableComponent
    {
        /// <summary>
        /// Makes a new instance of <see cref="Mixer"/> class.
        /// </summary>
        public Mixer()
        {
            Effects = new List<IEffect>();

            Channels = new Dictionary<string, List<IMixableComponent>>();
        }

        #region Properties

        private double VolumeInternal = 100D;
        private double PanInternal = 0D;
        private bool MuteInternal = false;

        /// <summary>
        /// Volume of this object.
        /// </summary>
        [Bindable]
        public double Volume
        {
            get { return VolumeInternal; }
            set
            {
                if (VolumeInternal == value)
                    return;

                VolumeInternal = value;

                UpdateState();
            }
        }

        /// <summary>
        /// Sound orientation.
        /// </summary>
        [Bindable]
        public double Pan
        {
            get { return PanInternal; }
            set
            {
                if (PanInternal == value)
                    return;

                UpdateState();
            }
        }

        /// <summary>
        /// Mute this element.
        /// </summary>
        [Bindable]
        public bool Mute
        {
            get { return MuteInternal; }
            set
            {
                if (MuteInternal == value)
                    return;

                MuteInternal = value;

                UpdateState();
            }
        }

        /// <summary>
        /// Real volume value.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public double RelativeVolume
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.RelativeVolume * (Volume / 100D) * (Parent.Mute ? 0 : 1);
                }
                else return Volume / 100D;
            }
        }

        /// <summary>
        /// Real Sound orientation.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public double RelativePan
        {
            get
            {
                if (Parent != null)
                {
                    return (Parent.RelativePan + (Pan / 100D)) / 2;
                }
                else return Pan / 100D;
            }
        }

        /// <summary>
        /// Hierarchy parent element.
        /// </summary>
        public IMixableComponent Parent { get; set; }

        #endregion

        #region Channel management

        private Dictionary<string, List<IMixableComponent>> Channels;

        /// <summary>
        /// Add a mixable chanel to this mixer.
        /// </summary>
        /// <param name="Elem">Mixable element.</param>
        /// <param name="Label">Channel label.</param>
        public void AddChannel(IMixableComponent Elem, string Label)
        {
            if (Elem == null)
                return;

            if (!Channels.ContainsKey(Label))
                Channels.Add(Label, new List<IMixableComponent>());

            Channels[Label].Add(Elem);
            Elem.Parent = this;
        }

        /// <summary>
        /// Remove an addd channel from this mixer.
        /// </summary>
        /// <param name="Label">Channel label.</param>
        public void RemoveChannel(string Label)
        {
            if (!Channels.ContainsKey(Label))
                return;

            foreach (IMixableComponent Component in Channels[Label])
            {
                BassMix.MixerRemoveChannel(Component.StreamHandler);
                Component.Parent = null;
            }

            Channels.Remove(Label);
        }
        
        /// <summary>
        /// Remove a component from a channel.
        /// </summary>
        /// <param name="Elem">Component to remove.</param>
        /// <param name="Label">Channel.</param>
        public void RemoveElementFromChannel(IMixableComponent Elem, string Label)
        {
            if (Channels[Label].Contains(Elem))
                Elem.Parent = null;
        }

        public IEnumerable<IMixableComponent> this[string Index]
        {
            get
            {
                return Channels[Index];
            }
        }

        #endregion

        #region Channels update

        public void UpdateState()
        {
            foreach (List<IMixableComponent> Elem in Channels.Values)
            {
                for (int i = 0; i < Elem.Count; ++i)
                {
                    if (Elem[i].Parent != this)
                        Elem.Remove(Elem[i]);
                    Elem[i].UpdateState();
                }
            }
        }

        #endregion

        #region Effects

        private List<IEffect> Effects;

        /// <summary>
        /// Add an effect to this object.
        /// </summary>
        /// <param name="Target">Effect.</param>
        public void AddEffect(IEffect Target, int Priority = 0)
        {
            if (Effects.Contains(Target))
                return;

            foreach (IMixableComponent Elem in Channels.Values)
            {
                Elem.AddEffect(Target, Priority);
            }

            Effects.Add(Target);
        }

        /// <summary>
        /// Remove an effect to this object.
        /// </summary>
        /// <param name="Target">Effect to remove.</param>
        public void RemoveEffect(IEffect Target)
        {
            if (!Effects.Contains(Target))
                return;

            foreach (IMixableComponent Elem in Channels.Values)
            {
                Elem.RemoveEffect(Target);
            }

            Effects.Remove(Target);
        }

        #endregion

        #region Handler

        /// <summary>
        /// Stream handler.
        /// </summary>
        public int StreamHandler => 0;

        #endregion
    }
}
