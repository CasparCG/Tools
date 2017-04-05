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

import com.casparcg.framework.server.Channel;
import com.casparcg.framework.server.Layer;
import com.casparcg.framework.server.producer.Route;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class Multiview {
    private final MultiviewLayout mLayout;
    private final List<Layer> mInputs = new ArrayList<>();
    private final List<Layer> mSources = new ArrayList<>();
    private final Layer mPreview;
    private final Layer mProgram;

    public Multiview(
            Channel multiviewChannel,
            Channel programChannel,
            Layer previewLayer,
            MultiviewLayout layout,
            int numInputs,
            boolean mipmap) {
        mLayout = layout;
        int numDecourLayers = mLayout.getNumDecourLayers(numInputs);

        mPreview = multiviewChannel.layer(numDecourLayers + 0);
        mProgram = multiviewChannel.layer(numDecourLayers + 1);

        for (int i = 0; i < numInputs; ++i) {
            mInputs.add(multiviewChannel.layer(numDecourLayers + 2 + i));
            mSources.add(null);
        }

        List<Layer> decourLayers = new ArrayList<>();

        for (int i = 0; i < numDecourLayers; ++i) {
            decourLayers.add(multiviewChannel.layer(i));
        }

        mLayout.performLayout(decourLayers, mPreview, mProgram, mInputs);

        mPreview.mipmapping().set(mipmap);
        mProgram.mipmapping().set(mipmap);
        mInputs.forEach(i -> i.mipmapping().set(mipmap));
        mPreview.play(new Route(previewLayer));
        mProgram.play(new Route(programChannel));
    }

    public void setupInput(int inputIndex, Layer sourceLayer) {
        mInputs.get(inputIndex).play(new Route(sourceLayer));
        mSources.set(inputIndex, sourceLayer);
    }

    public void programSelected(int inputIndex) {
        mLayout.program(mInputs.get(inputIndex));
    }

    public void previewSelected(int inputIndex) {
        mLayout.preview(mInputs.get(inputIndex));
    }
}
