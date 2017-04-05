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
package com.casparcg.tools.videomixer.client;

import java.io.IOException;
import java.util.Arrays;

import com.casparcg.framework.server.Easing;
import com.casparcg.tools.videomixer.Switcher;
import com.casparcg.tools.videomixer.TransitionType;
import com.illposed.osc.OSCMessage;
import com.illposed.osc.OSCPortOut;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class OscSwitcherProxy implements Switcher {
    private final OSCPortOut mPort;

    /**
     * Constructor.
     *
     */
    public OscSwitcherProxy(OSCPortOut port, int portNumber) {
        mPort = port;
        send("/switcher/subscribe", "localhost", portNumber);
    }

    private void send(String path, Object... params) {
        try {
            mPort.send(new OSCMessage(path, Arrays.asList(params)));
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    /** {@inheritDoc} */
    @Override
    public void preview(int inputId) {
        send("/switcher/preview/select", inputId);
    }

    /** {@inheritDoc} */
    @Override
    public void program(int inputId) {
        send("/switcher/program/select", inputId);
    }

    /** {@inheritDoc} */
    @Override
    public void cut() {
        send("/switcher/cut", 1);
    }

    /** {@inheritDoc} */
    @Override
    public void transitionType(TransitionType type) {
        send("/switcher/transition/type", type.name());
    }

    /** {@inheritDoc} */
    @Override
    public void take() {
        send("/switcher/take", 1);
    }

    /** {@inheritDoc} */
    @Override
    public void manualTransition(double lever) {
        send("/switcher/transition/lever", (float) lever);
    }

    /** {@inheritDoc} */
    @Override
    public void transitionDuration(int frames) {
        send("/switcher/transition/duration", frames);
    }

    /** {@inheritDoc} */
    @Override
    public void transitionEasing(Easing easing) {
        send("/switcher/transition/easing", easing.name());
    }
}
