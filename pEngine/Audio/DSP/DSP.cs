// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;

using pEngine.Framework;
using pEngine.Framework.Binding;

using ManagedBass;

namespace pEngine.Audio.DSP
{
    /// <summary>
    /// Base class for DSPs.
    /// </summary>
    public abstract class DSP : pObject, IEffect
    {

        public DSP() : base()
        {

        }

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public override void Dispose()
        {
            Bass.ChannelRemoveDSP(Channel, Handle);
            IsAssigned = false;
        }

        #region Properties

        private int priority;
        private bool assigned;
        private bool bypass;

        /// <summary>
        /// Gets the Channel on which the DSP is applied.
        /// </summary>
        public int Channel { get; private set; }

        /// <summary>
        /// Gets or Sets the DSP priority.
        /// </summary>
        [Bindable]
        public int Priority
        {
            get { return priority; }
            set
            {
                if (Bass.FXSetPriority(Handle, value))
                    priority = value;
            }
        }

        /// <summary>
        /// Gets whether the DSP is assigned.
        /// </summary>
        [Bindable]
        public bool IsAssigned
        {
            get { return assigned; }
            private set
            {
                assigned = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether the DSP should be bypassed.
        /// </summary>
        [Bindable]
        public bool Bypass
        {
            get { return bypass; }
            set
            {
                bypass = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Resolution"/> of the <see cref="Channel"/> on which the DSP is applied.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public Resolution Resolution { get; private set; }

        #endregion

        #region Handler

        private int Handle;

        public void BindStream(int Stream, int Priority)
        {
            Channel = Stream;
            priority = Priority;

            Handle = Bass.ChannelSetDSP(Channel, OnDsp, Priority: priority);

            Resolution = Bass.ChannelGetInfo(Channel).Resolution;

            Bass.ChannelSetSync(Channel, SyncFlags.Free, 0, (a, b, c, d) => Dispose());

            if (Handle != 0)
                IsAssigned = true;
            else throw new InvalidOperationException("DSP Assignment Failed");
        }

        public void UnbindStream(int Stream)
        {
            if (Stream != Channel)
                return;

            Bass.ChannelRemoveDSP(Channel, Handle);
            IsAssigned = false;
            Channel = 0;
            Handle = 0;
        }

        #endregion

        #region Callback

        void OnDsp(int handle, int channel, IntPtr Buffer, int Length, IntPtr User)
        {
            if (IsAssigned && !Bypass)
                Callback(Buffer, Length);
        }

        /// <summary>
        /// DSP Callback.
        /// </summary>
        /// <param name="Buffer">Pointer to Buffer allocated by Bass.</param>
        /// <param name="Length">No of bytes in buffer.</param>
        protected abstract void Callback(IntPtr Buffer, int Length);

        #endregion

    }
}
