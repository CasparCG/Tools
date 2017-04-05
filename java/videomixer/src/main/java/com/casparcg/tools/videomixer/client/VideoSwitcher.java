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

import java.util.ArrayList;
import java.util.List;

import com.casparcg.framework.server.Easing;
import com.casparcg.tools.videomixer.Switcher;
import com.casparcg.tools.videomixer.SwitcherListener;
import com.casparcg.tools.videomixer.TransitionType;

import javafx.geometry.Orientation;
import javafx.geometry.Pos;
import javafx.scene.control.Button;
import javafx.scene.control.ChoiceBox;
import javafx.scene.control.Slider;
import javafx.scene.control.Spinner;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class VideoSwitcher extends HBox implements SwitcherListener {
    private final List<Button> mPreview = new ArrayList<>();
    private final List<Button> mProgram = new ArrayList<>();
    private final Button mCut = new Button("Cut");
    private final Button mTake = new Button("Take");
    private final Slider mTransitionLever = new Slider(0.0, 1.0, 0.0);
    private final ChoiceBox<TransitionType> mTransitionType = new ChoiceBox<>();
    private final ChoiceBox<Easing> mTransitionEasing = new ChoiceBox<>();
    private final Spinner<Integer> mTransitionDuration = new Spinner<>(0, 1000, 0);

    private boolean mSuppress;

    /**
     * Constructor.
     *
     */
    public VideoSwitcher(int numInputs) {
        setSpacing(10);
        setAlignment(Pos.BOTTOM_LEFT);
        for (int i = 0; i < numInputs; ++i) {
            mPreview.add(new Button(String.valueOf(i + 1)));
            mProgram.add(new Button(String.valueOf(i + 1)));
        }

        mTransitionLever.setOrientation(Orientation.VERTICAL);
        mTransitionType.getItems().setAll(TransitionType.values());
        mTransitionEasing.getItems().setAll(Easing.values());
        mTransitionDuration.setEditable(true);

        HBox preview = new HBox();
        HBox program = new HBox();
        preview.getChildren().setAll(mPreview);
        program.getChildren().setAll(mProgram);
        VBox previewProgramBox = new VBox(program, preview);

        previewProgramBox.setAlignment(Pos.BOTTOM_LEFT);

        getChildren().add(previewProgramBox);

        getChildren().add(mCut);
        getChildren().add(mTake);
        VBox transitionBox = new VBox();
        transitionBox.setAlignment(Pos.BOTTOM_LEFT);
        transitionBox.getChildren().add(mTransitionType);
        transitionBox.getChildren().add(mTransitionEasing);
        transitionBox.getChildren().add(mTransitionDuration);
        getChildren().add(transitionBox);
        getChildren().add(mTransitionLever);
    }

    public void useSwitcher(Switcher switcher) {
        for (int i = 0; i < mPreview.size(); ++i) {
            final int inputId = i;
            mPreview.get(i).armedProperty().addListener((obj, o, n) -> {
                if (!o && n) {
                    switcher.preview(inputId);
                }
            });
            mProgram.get(i).armedProperty().addListener((obj, o, n) -> {
                if (!o && n) {
                    switcher.program(inputId);
                }
            });
        }

        mCut.armedProperty().addListener((obj, o, n) -> {
            if (!o && n) {
                switcher.cut();
            }
        });
        mTake.armedProperty().addListener((obj, o, n) -> {
            if (!o && n) {
                switcher.take();
            }
        });
        mTransitionLever.valueProperty().addListener((obj, o, n) -> {
            if (mSuppress) {
                return;
            }
            switcher.manualTransition(n.doubleValue());
        });
        mTransitionType.valueProperty().addListener((obj, o, n) -> {
            if (mSuppress) {
                return;
            }
            switcher.transitionType(n);
        });
        mTransitionEasing.valueProperty().addListener((obj, o, n) -> {
            if (mSuppress) {
                return;
            }
            switcher.transitionEasing(n);
        });
        mTransitionDuration.valueProperty().addListener((obj, o, n) -> {
            if (mSuppress) {
                return;
            }
            switcher.transitionDuration(n);
        });
    }

    /** {@inheritDoc} */
    @Override
    public void onPreviewSelected(int inputId, boolean onAir) {
        for (int i = 0; i < mPreview.size(); ++i) {
            if (i == inputId) {
                mPreview.get(i).setStyle("-fx-background-color: " + (onAir ? "red" : "green"));
            } else {
                mPreview.get(i).setStyle("");;
            }
        }
    }

    /** {@inheritDoc} */
    @Override
    public void onProgramSelected(int inputId) {
        for (int i = 0; i < mProgram.size(); ++i) {
            if (i == inputId) {
                mProgram.get(i).setStyle("-fx-background-color: red");
            } else {
                mProgram.get(i).setStyle("");;
            }
        }
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionLever(double lever) {
        mSuppress = true;
        mTransitionLever.setValue(lever);
        mSuppress = false;
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionType(TransitionType type) {
        mSuppress = true;
        mTransitionType.setValue(type);
        mSuppress = false;
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionDuration(int frames) {
        mSuppress = true;
        mTransitionDuration.getValueFactory().setValue(frames);
        mSuppress = false;
    }

    /** {@inheritDoc} */
    @Override
    public void onTransitionEasing(Easing easing) {
        mSuppress = true;
        mTransitionEasing.setValue(easing);
        mSuppress = false;
    }
}
