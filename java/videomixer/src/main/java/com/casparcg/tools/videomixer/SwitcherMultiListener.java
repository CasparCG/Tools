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
package com.casparcg.tools.videomixer;

import java.util.List;

import com.casparcg.framework.server.Easing;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class SwitcherMultiListener implements SwitcherListener {
    private final List<SwitcherListener> mListeners;

    /**
     * Constructor.
     *
     * @param listeners
     */
    public SwitcherMultiListener(List<SwitcherListener> listeners) {
        mListeners = listeners;
    }

    /** {@inheritDoc} */
    @Override
    public void onPreviewSelected(int inputId, boolean onAir) {
        mListeners.forEach(l -> l.onPreviewSelected(inputId, onAir));
    }

    /** {@inheritDoc} */
    @Override
    public void onProgramSelected(int inputId) {
        mListeners.forEach(l -> l.onProgramSelected(inputId));
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionLever(double lever) {
        mListeners.forEach(l -> l.onTransitionLever(lever));
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionType(TransitionType type) {
        mListeners.forEach(l -> l.onTransitionType(type));
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionDuration(int frames) {
        mListeners.forEach(l -> l.onTransitionDuration(frames));
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionEasing(Easing easing) {
        mListeners.forEach(l -> l.onTransitionEasing(easing));
    }
}
