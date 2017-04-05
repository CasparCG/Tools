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

import java.util.List;

import com.casparcg.framework.server.Layer;
import com.casparcg.framework.server.producer.BasicProducer;
import com.casparcg.framework.server.producer.Color;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class InputsAtBottomLayout implements MultiviewLayout {
    private static final String DECOUR_COLOR = "#CCCCCC";
    private static final String SELECTED_PROGRAM_COLOR = "RED";
    private static final String SELECTED_PREVIEW_COLOR = "GREEN";
    private static final double DECOUR = 0.005;

    private Layer mSelectedProgramLayer;
    private Layer mSelectedPreviewLayer;

    /** {@inheritDoc} */
    @Override
    public int getNumDecourLayers(int inputs) {
        return 1 + 2 + 2 + 2 + inputs;
    }

    @SuppressWarnings("restriction")
    private static void moveTo(Layer toMove, Layer destination) {
        toMove.fill().modify(
                destination.fill().positionX().get(),
                destination.fill().positionY().get(),
                destination.fill().scaleX().get(),
                destination.fill().scaleY().get());
    }

    /** {@inheritDoc} */
    @Override
    public void performLayout(
            List<Layer> decourLayers,
            Layer preview,
            Layer program,
            List<Layer> inputs) {
        Layer decourLayer = decourLayers.get(0);
        Layer programBackgroundLayer = decourLayers.get(1);
        Layer previewBackgroundLayer = decourLayers.get(2);
        Layer restBottomLayer = decourLayers.get(3);
        Layer restSideLayer = decourLayers.get(4);
        mSelectedPreviewLayer = decourLayers.get(5);
        mSelectedProgramLayer = decourLayers.get(6);

        preview.fill().modify(0 + DECOUR,         DECOUR, 0.5 - 1.5 * DECOUR, 0.5 - 1.5 * DECOUR);
        program.fill().modify(0.5 + 0.5 * DECOUR, DECOUR, 0.5 - 1.5 * DECOUR, 0.5 - 1.5 * DECOUR);

        moveTo(previewBackgroundLayer, preview);
        moveTo(programBackgroundLayer, program);

        previewBackgroundLayer.play(Color.BLACK);
        programBackgroundLayer.play(Color.BLACK);

        int numberOfInputs = inputs.size();
        double inputScale = 0.5 - 3 * DECOUR / 2;

        if (numberOfInputs == 3) {
            inputScale = 1.0 / 3.0 - 4 * DECOUR / 3;
        } else if (numberOfInputs >= 4) {
            inputScale = 1.0 / 4.0 - 5 * DECOUR / 4;
        }

        double x = DECOUR;
        double y = 0.5 + 0.5 * DECOUR;

        for (int i = 0; i < inputs.size(); ++i) {
            Layer input = inputs.get(i);
            Layer inputBackground = decourLayers.get(7 + i);

            if (x > 0.98) {
                x = DECOUR;
                y += inputScale + DECOUR;
            }

            input.fill().modify(x, y, inputScale, inputScale);
            moveTo(inputBackground, input);
            x += inputScale + DECOUR;

            inputBackground.play(Color.BLACK);
        }

        if (x != DECOUR) {
            restSideLayer.fill().position(x, y);
            restSideLayer.play(Color.BLACK);
            y += inputScale + DECOUR;
        }

        if (y < 1.0) {
            restBottomLayer.fill().positionY().set(y);
            restBottomLayer.play(Color.BLACK);
        }

        decourLayer.play(new BasicProducer(DECOUR_COLOR));

        preview(inputs.get(1));
        program(inputs.get(0));

        mSelectedPreviewLayer.play(new BasicProducer(SELECTED_PREVIEW_COLOR));
        mSelectedProgramLayer.play(new BasicProducer(SELECTED_PROGRAM_COLOR));
    }

    private static void moveSelectionTo(Layer selectionLayer, Layer inputLayer) {
        selectionLayer.fill().modify(
                inputLayer.fill().positionX().get() - DECOUR,
                inputLayer.fill().positionY().get() - DECOUR,
                inputLayer.fill().scaleX().get() + 2 * DECOUR,
                inputLayer.fill().scaleY().get() + 2 * DECOUR);
    }

    /** {@inheritDoc} */
    @Override
    public void preview(Layer input) {
        moveSelectionTo(mSelectedPreviewLayer, input);
    }

    /** {@inheritDoc} */
    @Override
    public void program(Layer input) {
        moveSelectionTo(mSelectedProgramLayer, input);
    }
}
