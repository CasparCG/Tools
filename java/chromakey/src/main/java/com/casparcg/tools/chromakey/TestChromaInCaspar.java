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
package com.casparcg.tools.chromakey;

import java.util.function.Function;

import com.casparcg.framework.client.rundown.CustomCommandItem;
import com.casparcg.framework.client.rundown.Rundown;
import com.casparcg.framework.server.CasparDevice;
import com.casparcg.framework.server.ChromaKey;
import com.casparcg.framework.server.Layer;
import com.casparcg.framework.server.amcp.AmcpCasparDeviceFactory;
import com.thoughtworks.xstream.XStream;

import javafx.application.Application;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.geometry.Insets;
import javafx.geometry.Orientation;
import javafx.scene.Node;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.CheckBox;
import javafx.scene.control.Label;
import javafx.scene.control.Separator;
import javafx.scene.control.Slider;
import javafx.scene.control.Spinner;
import javafx.scene.control.TextField;
import javafx.scene.input.Clipboard;
import javafx.scene.input.ClipboardContent;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.Priority;
import javafx.stage.Stage;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class TestChromaInCaspar extends Application {

    private GridPane mRoot;
    private int mRow;

    /** {@inheritDoc} */
    @Override
    public void start(Stage primaryStage) throws Exception {
        TextField casparName = new TextField("Local CasparCG");
        TextField casparHost = new TextField("127.0.0.1");
        Spinner<Integer> casparPort = new Spinner<>(0, 65535, 5250);
        Spinner<Integer> casparChannel = new Spinner<>(1, Integer.MAX_VALUE, 1);
        Spinner<Integer> casparLayer = new Spinner<>(0, Integer.MAX_VALUE, 10);
        Button connect = new Button("Connect");

        Slider hueSlider = new Slider(0, 360, 120);
        Slider hueWidthSlider = new Slider(0, 1, 0.1);
        Slider saturationSlider = new Slider(0, 1, 0);
        Slider brightnessSlider = new Slider(0, 1, 0);

        Slider softnessSlider = new Slider(0, 1, 0.1);
        Slider spillSuppressionSlider = new Slider(0, 180.0, 0);
        Slider spillSuppressionSaturationSlider = new Slider(0, 1, 1);
        CheckBox showMaskCheckBox = new CheckBox("Show mask");

        Button rundownToClipboard = new Button("Rundown to clipboard");

        Function<Node, Node> grow = (child) -> {
            GridPane.setHgrow(child, Priority.ALWAYS);
            return child;
        };

        mRoot = new GridPane();
        mRoot.setPadding(new Insets(5));
        mRoot.setHgap(5);
        mRoot.setVgap(10);

        add("CasparCG Server Name in Client:", casparName);
        add("CasparCG Server Host:", casparHost);
        add("CasparCG Server Port:", casparPort);
        add("Layer:", casparLayer);
        add("", connect);

        addSeperator();

        add("Hue:", grow.apply(hueSlider));
        add("Hue Width:", grow.apply(hueWidthSlider));
        add("Minimum Saturation:", grow.apply(saturationSlider));
        add("Minimum Brightness:", grow.apply(brightnessSlider));

        addSeperator();

        add("Softness:", grow.apply(softnessSlider));
        add("Spill Suppression:", grow.apply(spillSuppressionSlider));
        add("Spill Suppression Saturation:", grow.apply(spillSuppressionSaturationSlider));
        add("", grow.apply(showMaskCheckBox));

        addSeperator();

        add("", rundownToClipboard);

        Scene scene = new Scene(mRoot);
        primaryStage.setTitle("Chroma Key");
        primaryStage.setWidth(600);
        primaryStage.setHeight(460);
        primaryStage.setScene(scene);
        primaryStage.show();

        ObjectProperty<ChromaKey> chroma = new SimpleObjectProperty<>();

        chroma.addListener((o, old, c) -> {
            if (old != null) {
                old.targetHue().unbind();
                old.hueWidth().unbind();
                old.minSaturation().unbind();
                old.minBrightness().unbind();
                old.softness().unbind();
                old.spillSuppress().unbind();
                old.spillSuppressSaturation().unbind();
                old.showMask().unbind();
            }

            c.targetHue().bind(hueSlider.valueProperty());
            c.hueWidth().bind(hueWidthSlider.valueProperty());
            c.minSaturation().bind(saturationSlider.valueProperty());
            c.minBrightness().bind(brightnessSlider.valueProperty());
            c.softness().bind(softnessSlider.valueProperty());
            c.spillSuppress().bind(spillSuppressionSlider.valueProperty());
            c.spillSuppressSaturation().bind(spillSuppressionSaturationSlider.valueProperty());
            c.showMask().bind(showMaskCheckBox.selectedProperty());
            c.enable().set(true);
        });

        ObjectProperty<CasparDevice> device = new SimpleObjectProperty<>();

        device.addListener((o, old, d) -> {
            if (old != null)
                old.close();
        });

        connect.setOnAction(e -> {
            try {
                device.set(new AmcpCasparDeviceFactory().create(casparHost.getText(), casparPort.getValue()));
            } catch (RuntimeException ex) {
                device.set(null);
                chroma.set(null);
            }
            Layer layer = device.get().channel(casparChannel.getValue()).layer(casparLayer.getValue());

            chroma.set(layer.chromaKey());
        });

        rundownToClipboard.setOnAction(e -> {
            Rundown rundown = new Rundown();

            rundown.add(new CustomCommandItem()
                    .deviceName(casparName.getText())
                    .label("Custom Chroma Key")
                    .stopCommand("MIXER " + casparChannel.getValue() + "-" + casparLayer.getValue() + " CHROMA 0")
                    .playCommand("MIXER " + casparChannel.getValue() + "-" + casparLayer.getValue() + " CHROMA 1"
                            + " " + hueSlider.getValue()
                            + " " + hueWidthSlider.getValue()
                            + " " + saturationSlider.getValue()
                            + " " + brightnessSlider.getValue()
                            + " " + softnessSlider.getValue()
                            + " " + spillSuppressionSlider.getValue()
                            + " " + spillSuppressionSaturationSlider.getValue()
                            + " " + (showMaskCheckBox.isSelected() ? "1" : "0")));

            ClipboardContent content = new ClipboardContent();
            XStream xstream = new XStream();
            xstream.autodetectAnnotations(true);
            content.putString(xstream.toXML(rundown));
            Clipboard.getSystemClipboard().setContent(content);
        });
    }

    public void add(String label, Node node) {
        int rowIndex = mRow++;
        mRoot.add(new Label(label), 0, rowIndex);
        mRoot.add(node, 1, rowIndex);
    }

    public void addSeperator() {
        Separator separator = new Separator(Orientation.HORIZONTAL);
        GridPane.setFillHeight(separator, true);
        mRoot.add(separator, 0, mRow++, 2, 1);
    }

    public static void main(String[] args) {
        launch(args);
    }
}
