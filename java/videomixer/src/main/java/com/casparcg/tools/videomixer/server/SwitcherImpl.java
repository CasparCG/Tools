/*
* Copyright (c) 2011 Sveriges Television AB <info@casparcg.com>
*
* This file is part of CasparCG (www.casparcg.com).
*
* CasparCG is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* CasparCG is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with CasparCG. If not, see <http://www.gnu.org/licenses/>.
*
* Author: Helge Norberg
*/
package com.casparcg.tools.videomixer.server;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;

import com.casparcg.framework.server.Channel;
import com.casparcg.framework.server.Easing;
import com.casparcg.framework.server.Layer;
import com.casparcg.framework.server.producer.Route;
import com.casparcg.tools.videomixer.Switcher;
import com.casparcg.tools.videomixer.SwitcherListener;
import com.casparcg.tools.videomixer.TransitionType;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class SwitcherImpl implements Switcher {
    private final SwitcherListener mListener;
    private final Scheduler mScheduler;
    private final Channel mProgramChannel;
    private final Multiview mMultiview;
    private final Layer mPreviewLayer;
    private final Layer mProgramLayer;
    private final List<Layer> mInputs = new ArrayList<>();

    private int mProgramSelected;
    private int mPreviewSelected;
    private Easing mTransitionEasing = Easing.linear;
    private int mTransitionDuration = 75;
    private TransitionType mTransitionType;
    private TransitionStrategy mTransitionStrategy = new MixTransitionStrategy();
    private double mTransitionLever;
    private boolean mTransitionInProgress;
    private boolean mLeverInvert;

    public SwitcherImpl(
            Channel programChannel,
            Channel multiviewChannel,
            int numInputs,
            MultiviewLayout layout,
            boolean mipmap,
            SwitcherListener listener,
            Scheduler scheduler) {
        mListener = listener;
        mScheduler = scheduler;
        mProgramChannel = programChannel;
        mProgramLayer = programChannel.layer(8);
        mPreviewLayer = programChannel.layer(9);
        mMultiview = new Multiview(
                multiviewChannel,
                programChannel,
                mPreviewLayer,
                layout,
                numInputs,
                mipmap);

        for (int i = 0; i < numInputs; ++i) {
            Layer input = mProgramChannel.layer(i + 10);
            mInputs.add(input);
            mMultiview.setupInput(i, input);
        }

        mPreviewLayer.adjustments().opacity().set(0.0);
        transitionType(TransitionType.MIX);
        hideAll();
        preview(0);
        program(0);
        publishState();
    }

    private void hideAll() {
        mInputs.forEach(input -> input.adjustments().opacity().set(0.0));
    }

    /** {@inheritDoc} */
    @Override
    public void preview(int inputId) {
        mPreviewLayer.play(new Route(mInputs.get(inputId)));

        mPreviewSelected = inputId;
        mMultiview.previewSelected(mPreviewSelected);
        mListener.onPreviewSelected(mPreviewSelected, mTransitionInProgress);
    }

    /** {@inheritDoc} */
    @Override
    public void program(int inputId) {
        mProgramLayer.play(new Route(mInputs.get(inputId)));
        mProgramSelected = inputId;
        mMultiview.programSelected(mProgramSelected);
        mListener.onProgramSelected(mProgramSelected);
    }

    private void swapSelected() {
        int temp = mProgramSelected;
        mProgramSelected = mPreviewSelected;
        mPreviewSelected = temp;

        mMultiview.previewSelected(mPreviewSelected);
        mMultiview.programSelected(mProgramSelected);
        mListener.onPreviewSelected(mPreviewSelected, mTransitionInProgress);
        mListener.onProgramSelected(mProgramSelected);
    }

    /** {@inheritDoc} */
    @Override
    public void cut() {
        // swap
        mPreviewLayer.swap(mProgramLayer, false);
        swapSelected();
    }

    /** {@inheritDoc} */
    @Override
    public void transitionType(TransitionType type) {
        mTransitionStrategy.reset();
        switch (type) {
        case MIX:
            mTransitionStrategy = new MixTransitionStrategy();
            break;
        case CUT:
            mTransitionStrategy = new CutTransitionStrategy();
            break;
        case PUSH:
            mTransitionStrategy = new PushTransitionStrategy();
            break;
        case ZOOM_AND_ROTATE:
            mTransitionStrategy = new ZoomRotateTransitionStragegy();
            break;
        default:
            throw new IllegalArgumentException("Unhandled type: " + type);
        }

        mTransitionType = type;
        mListener.onTransitionType(mTransitionType);
        if (mTransitionLever > 0.0 && mTransitionLever < 1.0) {
            manualTransition(mTransitionLever);
        }
    }

    /** {@inheritDoc} */
    @Override
    public void transitionDuration(int frames) {
        mTransitionDuration = frames;
        mListener.onTransitionDuration(frames);
    }

    /** {@inheritDoc} */
    @Override
    public void transitionEasing(Easing easing) {
        mTransitionEasing = easing;
        mListener.onTransitionEasing(easing);
    }

    /** {@inheritDoc} */
    @Override
    public void take() {
        if (mTransitionInProgress) {
            return;
        }

        mListener.onPreviewSelected(mPreviewSelected, true);
        mTransitionStrategy.auto();
        mTransitionInProgress = true;

        long sleepForMillis =
                (long) (mProgramChannel.videoMode().getMillisecondsPerFrame()
                        * (mTransitionDuration + 1));

        if (mProgramChannel.videoMode().isInterlaced())
            sleepForMillis /= 2;

        mScheduler.schedule(
                TimeUnit.MILLISECONDS, sleepForMillis, this::endTransition);
    }

    /** {@inheritDoc} */
    @Override
    public void manualTransition(double lever) {
        if (mTransitionInProgress) {
            return;
        }

        double transitionAmount = mLeverInvert ? 1.0 - lever : lever;

        if (transitionAmount == 1.0) {
            endTransition();
            return;
        }

        mTransitionStrategy.manual(transitionAmount);
        mTransitionLever = lever;
        mListener.onTransitionLever(lever);
        mListener.onPreviewSelected(mPreviewSelected, transitionAmount != 0.0);
    }

    public void publishState() {
        mListener.onPreviewSelected(mPreviewSelected, mTransitionInProgress);
        mListener.onProgramSelected(mProgramSelected);
        mListener.onTransitionType(mTransitionType);
        mListener.onTransitionLever(mTransitionLever);
        mListener.onTransitionDuration(mTransitionDuration);
        mListener.onTransitionEasing(mTransitionEasing);
    }


    private void endTransition() {
        mProgramLayer.adjustments().opacity().easing(Easing.linear, 0);
        mProgramLayer.adjustments().opacity().set(1.0);
        mProgramLayer.swap(mPreviewLayer, false);
        mTransitionStrategy.reset();
        mListener.onTransitionLever(mLeverInvert ? 0.0 : 1.0);
        mLeverInvert = !mLeverInvert;
        mTransitionInProgress = false;
        swapSelected();
    }

    private interface TransitionStrategy {
        void manual(double transitionAmount);
        void auto();
        void reset();
    }

    private class CutTransitionStrategy implements TransitionStrategy {
        /** {@inheritDoc} */
        @Override
        public void manual(double transitionAmount) {
        }

        /** {@inheritDoc} */
        @Override
        public void auto() {
        }

        /** {@inheritDoc} */
        @Override
        public void reset() {
        }
    }

    private class MixTransitionStrategy implements TransitionStrategy {
        /** {@inheritDoc} */
        @Override
        public void manual(double transitionAmount) {
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().set(transitionAmount);
        }

        /** {@inheritDoc} */
        @Override
        public void auto() {
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().set(0.0);
            mPreviewLayer.adjustments().opacity().easing(mTransitionEasing, mTransitionDuration);
            mPreviewLayer.adjustments().opacity().set(1.0);
        }

        /** {@inheritDoc} */
        @Override
        public void reset() {
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().set(0.0);
        }
    }

    private class PushTransitionStrategy implements TransitionStrategy {
        /** {@inheritDoc} */
        @Override
        public void manual(double transitionAmount) {
            mPreviewLayer.fill().easing(Easing.linear, 0);
            mProgramLayer.fill().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.fill().defer(true);
            mProgramLayer.fill().defer(true);
            mPreviewLayer.adjustments().opacity().set(1.0);
            mPreviewLayer.fill().positionX().set(transitionAmount - 1.0);
            mProgramLayer.fill().positionX().set(transitionAmount);
            mProgramChannel.commitDeffered();
            mProgramLayer.fill().defer(false);
            mPreviewLayer.fill().defer(false);
        }

        /** {@inheritDoc} */
        @Override
        public void auto() {
            mPreviewLayer.fill().easing(Easing.linear, 0);
            mPreviewLayer.fill().positionX().set(-1.0);
            mProgramLayer.fill().easing(Easing.linear, 0);
            mProgramLayer.fill().positionX().set(0.0);
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().set(1.0);
            mPreviewLayer.fill().easing(mTransitionEasing, mTransitionDuration);
            mProgramLayer.fill().easing(mTransitionEasing, mTransitionDuration);
            mPreviewLayer.fill().defer(true);
            mProgramLayer.fill().defer(true);
            mPreviewLayer.fill().positionX().set(0.0);
            mProgramLayer.fill().positionX().set(1.0);
            mProgramChannel.commitDeffered();
       }

        /** {@inheritDoc} */
        @Override
        public void reset() {
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().set(0.0);
            mProgramLayer.fill().defer(false);
            mPreviewLayer.fill().defer(false);
            mProgramLayer.fill().easing(Easing.linear, 0);
            mProgramLayer.fill().positionX().set(0.0);
            mPreviewLayer.fill().easing(Easing.linear, 0);
            mPreviewLayer.fill().positionX().set(0.0);
        }
    }

    private class ZoomRotateTransitionStragegy implements TransitionStrategy {
        private final double mFromX = 0.2;
        private final double mFromY = 0.2;
        private final double mFromAngle = 720;

        private void prepare() {
            mPreviewLayer.anchorPoint().defer(true);
            mPreviewLayer.fill().defer(true);
            mPreviewLayer.rotation().defer(true);
            mPreviewLayer.adjustments().opacity().defer(true);

            mPreviewLayer.anchorPoint().position(0.5, 0.5);
            mPreviewLayer.fill().modify(mFromX, mFromY, 0.0, 0.0);
            mPreviewLayer.rotation().set(mFromAngle);
            mPreviewLayer.adjustments().opacity().set(0);
        }

        /** {@inheritDoc} */
        @Override
        public void manual(double transitionAmount) {
            prepare();
            mPreviewLayer.rotation().set(mFromAngle - transitionAmount * mFromAngle);
            double deltaX = 0.5 - mFromX;
            double deltaY = 0.5 - mFromY;
            mPreviewLayer.fill().modify(
                    mFromX + deltaX * transitionAmount,
                    mFromY + deltaY * transitionAmount,
                    transitionAmount,
                    transitionAmount);
            mPreviewLayer.adjustments().opacity().set(transitionAmount);
            mProgramChannel.commitDeffered();
            mPreviewLayer.rotation().defer(false);
            mPreviewLayer.fill().defer(false);
            mPreviewLayer.anchorPoint().defer(false);
            mPreviewLayer.adjustments().opacity().defer(false);
        }

        /** {@inheritDoc} */
        @Override
        public void auto() {
            prepare();
            mProgramChannel.commitDeffered();
            mPreviewLayer.rotation().easing(mTransitionEasing, mTransitionDuration);
            mPreviewLayer.rotation().set(0.0);
            mPreviewLayer.fill().easing(mTransitionEasing, mTransitionDuration);
            mPreviewLayer.fill().modify(0.5, 0.5, 1.0, 1.0);
            mPreviewLayer.adjustments().opacity().easing(mTransitionEasing, mTransitionDuration);
            mPreviewLayer.adjustments().opacity().set(1.0);
            mProgramChannel.commitDeffered();
            mPreviewLayer.rotation().defer(false);
            mPreviewLayer.fill().defer(false);
            mPreviewLayer.anchorPoint().defer(false);
            mPreviewLayer.adjustments().opacity().defer(false);
        }

        /** {@inheritDoc} */
        @Override
        public void reset() {
            mPreviewLayer.adjustments().opacity().easing(Easing.linear, 0);
            mPreviewLayer.adjustments().opacity().set(0.0);

            mPreviewLayer.rotation().easing(Easing.linear, 0);
            mPreviewLayer.fill().easing(Easing.linear, 0);
            mPreviewLayer.anchorPoint().easing(Easing.linear, 0);

            mPreviewLayer.rotation().set(0);
            mPreviewLayer.fill().modify(0, 0, 1, 1);
            mPreviewLayer.anchorPoint().position(0, 0);
        }
    }
}
