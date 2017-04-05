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

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.PrintStream;
import java.net.InetAddress;
import java.net.SocketException;
import java.util.Properties;
import java.util.function.Function;

import com.casparcg.framework.server.Easing;
import com.casparcg.framework.server.osc.MultiOscHandler;
import com.casparcg.tools.videomixer.Switcher;
import com.casparcg.tools.videomixer.SwitcherListener;
import com.casparcg.tools.videomixer.TransitionType;
import com.illposed.osc.OSCPortIn;
import com.illposed.osc.OSCPortOut;
import com.illposed.osc.utility.JavaRegexAddressSelector;

import javafx.application.Application;
import javafx.application.Platform;
import javafx.scene.Scene;
import javafx.stage.Stage;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class SwitcherClient extends Application {
    /** {@inheritDoc} */
    @Override
    public void start(Stage primaryStage) throws Exception {
        Properties properties = new Properties();

        try {
            try (FileReader reader = new FileReader("mixerclient.properties")) {
                properties.load(reader);
            }
        } catch (FileNotFoundException e) {
            System.err.print("Missing mixerclient.properties. Trying to generate skeleton file... ");

            try (PrintStream out = new PrintStream("mixerclient.properties")) {
                out.println("mixerhost = localhost");
                out.println("mixeroscport = 7250");
                out.println("numinputs = 6");
                out.flush();
            }

            System.err.println("done. Please edit file and try again.");

            System.exit(1);
        }

        Function<String, String> require = prop -> {
            String value = properties.getProperty(prop);

            if (value == null) {
                throw new RuntimeException("mixerclient.properties is missing required property " + prop);
            }

            return value;
        };

        InetAddress mixerHost = InetAddress.getByName(require.apply("mixerhost"));
        int mixerOscPort = Integer.parseInt(require.apply("mixeroscport"));

        VideoSwitcher gui = new VideoSwitcher(6);
        int udpListenPort = 10000;
        OSCPortIn in = null;

        for (int i = 0; i < 10; ++i) {
            try {
                in = new OSCPortIn(++udpListenPort);
                break;
            } catch (SocketException e) {
                continue;
            }
        }

        Switcher switcher = new OscSwitcherProxy(new OSCPortOut(mixerHost, mixerOscPort), udpListenPort);
        gui.useSwitcher(switcher);

        SwitcherListener listener = gui;
        MultiOscHandler router = new MultiOscHandler();
        in.addListener(
                new JavaRegexAddressSelector(".*"),
                (d, m) -> Platform.runLater(() -> router.handle(m.getAddress(), m.getArguments())));
        in.startListening();

        router.subscribe("/switcher/preview/selected/", (path, args) -> {
            int state = (int) args.get(0);

            if (state == 1) {
                listener.onPreviewSelected(Integer.parseInt(path), false);
            }
        });
        router.subscribe("/switcher/preview/onair/", (path, args) -> {
            int state = (int) args.get(0);

            if (state == 1) {
                listener.onPreviewSelected(Integer.parseInt(path), true);
            }
        });
        router.subscribe("/switcher/program/selected/", (path, args) -> {
            int state = (int) args.get(0);

            if (state == 1) {
                listener.onProgramSelected(Integer.parseInt(path));
            }
        });
        router.subscribe("/switcher/transition/lever", (path, args) -> {
            float lever = (float) args.get(0);

            listener.onTransitionLever(lever);
        });
        router.subscribe("/switcher/transition/type", (path, args) -> {
            String type = (String) args.get(0);

            listener.onTransitionType(TransitionType.valueOf(type));
        });
        router.subscribe("/switcher/transition/easing", (path, args) -> {
            String easing = (String) args.get(0);

            listener.onTransitionEasing(Easing.valueOf(easing));
        });
        router.subscribe("/switcher/transition/duration", (path, args) -> {
            int frames = (int) args.get(0);

            listener.onTransitionDuration(frames);
        });

        Scene scene = new Scene(gui);
        scene.getStylesheets().add(getClass().getPackage().getName().replace('.', '/') + "/style.css");
        primaryStage.setScene(scene);
        primaryStage.sizeToScene();
        primaryStage.setTitle("Caspar VideoMixer");
        OSCPortIn inFinal = in;
        primaryStage.setOnCloseRequest(e -> {
            inFinal.stopListening();
            inFinal.close();
            Platform.exit();
        });
        primaryStage.show();
    }

    public static void main(String[] args) throws IOException {
        launch(args);
    }
}
