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

import java.util.Arrays;
import java.util.List;

import com.casparcg.framework.server.Easing;
import com.casparcg.tools.videomixer.SwitcherListener;
import com.casparcg.tools.videomixer.TransitionType;
import com.illposed.osc.OSCMessage;
import com.illposed.osc.OSCPortOut;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class ReportViaOSCListener implements SwitcherListener {
    private final List<OSCPortOut> mPorts;
    private final int mNumInputs;

    public ReportViaOSCListener(List<OSCPortOut> ports, int numInputs) {
        mPorts = ports;
        mNumInputs = numInputs;
    }

    private void send(String path, Object... params) {
        mPorts.forEach(p -> {
            try {
                p.send(new OSCMessage(path, Arrays.asList(params)));
            } catch (Exception e) {
                e.printStackTrace();
            }
        });
    }

    private void enableAndDisableRest(String prefix, int inputToEnable) {
        for (int i = 0; i < mNumInputs; ++i) {
            send(prefix + i, inputToEnable == i ? 1 : 0);
        }
    }

    /** {@inheritDoc} */
    @Override
    public void onPreviewSelected(int inputId, boolean onAir) {
        if (onAir) {
            enableAndDisableRest("/switcher/preview/onair/", inputId);
        } else {
            enableAndDisableRest("/switcher/preview/selected/", inputId);
        }
    }

    /** {@inheritDoc} */
    @Override
    public void onProgramSelected(int inputId) {
        enableAndDisableRest("/switcher/program/selected/", inputId);
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionLever(double lever) {
        send("/switcher/transition/lever", (float) lever);
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionType(TransitionType type) {
        send("/switcher/transition/type", type.name());
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionDuration(int frames) {
        send("/switcher/transition/duration", frames);
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionEasing(Easing easing) {
        send("/switcher/transition/easing", easing.name());
    }
}
