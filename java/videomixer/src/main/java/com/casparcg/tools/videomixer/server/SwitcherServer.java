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

import java.io.Closeable;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.PrintStream;
import java.net.InetSocketAddress;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Properties;
import java.util.Set;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;
import java.util.function.Function;

import com.casparcg.framework.server.CasparDevice;
import com.casparcg.framework.server.Easing;
import com.casparcg.framework.server.amcp.AmcpCasparDeviceFactory;
import com.casparcg.framework.server.osc.MultiOscHandler;
import com.casparcg.tools.videomixer.TransitionType;
import com.illposed.osc.OSCPortIn;
import com.illposed.osc.OSCPortOut;
import com.illposed.osc.utility.JavaRegexAddressSelector;

/**
 * TODO documentation.
 *
 * @author Helge Norberg, helge.norberg@svt.se
 */
public class SwitcherServer implements Closeable {
    private final boolean MIPMAP = true;

    private final CasparDevice mCasparServer;
    private final ScheduledExecutorService mExecutorService =
            Executors.newScheduledThreadPool(1);
    private final SwitcherImpl mSwitcher;
    private final OSCPortIn mOscSocket;

    /**
     * Constructor.
     *
     */
    public SwitcherServer(
            InetSocketAddress casparServer,
            int programChannel,
            int multiviewChannel,
            MultiviewLayout multiview,
            int numInputs,
            int oscPort) {
        mCasparServer = new AmcpCasparDeviceFactory().create(
                casparServer.getHostName(), casparServer.getPort());

        List<OSCPortOut> subscriberPorts = new ArrayList<>();
        mSwitcher = new SwitcherImpl(
                mCasparServer.channel(programChannel),
                mCasparServer.channel(multiviewChannel),
                numInputs,
                multiview,
                MIPMAP,
                new ReportViaOSCListener(subscriberPorts, numInputs),
                (unit, delay, command) -> mExecutorService.schedule(command, delay, unit));
        try {
            mOscSocket = new OSCPortIn(oscPort);
        } catch (SocketException e) {
            throw new RuntimeException(e);
        }

        MultiOscHandler router = new MultiOscHandler();
        mOscSocket.addListener(
                new JavaRegexAddressSelector(".*"),
                (d, m) -> mExecutorService.execute(() -> router.handle(m.getAddress(), m.getArguments())));
        mOscSocket.startListening();

        Set<InetSocketAddress> subscribers = new HashSet<>();
        router.subscribe("/switcher/subscribe", (path, args) -> {
            String address = (String) args.get(0);
            int port = (int) args.get(1);
            InetSocketAddress subscriber = new InetSocketAddress(address, port);

            if (subscribers.add(subscriber)) {
                try {
                    subscriberPorts.add(new OSCPortOut(subscriber.getAddress(), subscriber.getPort()));
                } catch (Exception e) {
                    throw new RuntimeException(e);
                }
            }

            mSwitcher.publishState();
        });
        router.subscribe("/switcher/preview/select", (path, args) -> {
            int inputId = (int) args.get(0);
            mSwitcher.preview(inputId);
        });
        router.subscribe("/switcher/program/select", (path, args) -> {
            int inputId = (int) args.get(0);
            mSwitcher.program(inputId);
        });
        router.subscribe("/switcher/cut", (path, args) -> {
            int state = (int) args.get(0);

            if (state == 1) {
                mSwitcher.cut();
            }
        });
        router.subscribe("/switcher/take", (path, args) -> {
            int state = (int) args.get(0);

            if (state == 1) {
                mSwitcher.take();
            }
        });
        router.subscribe("/switcher/transition/lever", (path, args) -> {
            float lever = (float) args.get(0);

            mSwitcher.manualTransition(lever);
        });
        router.subscribe("/switcher/transition/type", (path, args) -> {
            String type = (String) args.get(0);

            mSwitcher.transitionType(TransitionType.valueOf(type));
        });
        router.subscribe("/switcher/transition/duration", (path, args) -> {
            int frames = (int) args.get(0);

            mSwitcher.transitionDuration(frames);
        });
        router.subscribe("/switcher/transition/easing", (path, args) -> {
            String easing = (String) args.get(0);

            mSwitcher.transitionEasing(Easing.valueOf(easing));
        });
    }

    /** {@inheritDoc} */
    @Override
    public void close() {
        mOscSocket.stopListening();
        mOscSocket.close();
        mExecutorService.shutdownNow();

        try {
            mExecutorService.awaitTermination(10L, TimeUnit.SECONDS);
        } catch (InterruptedException e) {
        }

        mCasparServer.close();
    }

    public static void main(String[] args) throws IOException {
        Properties properties = new Properties();

        try {
            try (FileReader reader = new FileReader("mixerserver.properties")) {
                properties.load(reader);
            }
        } catch (FileNotFoundException e) {
            System.err.print("Missing mixerserver.properties. Trying to generate skeleton file... ");

            try (PrintStream out = new PrintStream("mixerserver.properties")) {
                out.println("casparhost = localhost");
                out.println("casparport = 5250");
                out.println("programchannel = 1");
                out.println("multiviewchannel = 2");
                out.println("# It is up to the user to play the content to be considered as input 1 up to [numinputs].");
                out.println("# The layer starts at 10 and goes up to 10 + [numinputs] - 1 on the [programchannel].");
                out.println("numinputs = 6");
                out.println("osclistenport = 7250");
                out.flush();
            }

            System.err.println("done. Please edit file and try again.");

            System.exit(1);
        }

        Function<String, String> require = prop -> {
            String value = properties.getProperty(prop);

            if (value == null) {
                throw new RuntimeException("mixerserver.properties is missing required property " + prop);
            }

            return value;
        };

        InetSocketAddress casparServer = new InetSocketAddress(
                require.apply("casparhost"),
                Integer.parseInt(require.apply("casparport")));
        int programChannel = Integer.parseInt(require.apply("programchannel"));
        int multiviewChannel = Integer.parseInt(require.apply("multiviewchannel"));
        MultiviewLayout multiview = new InputsAtBottomLayout();
        int numInputs = Integer.parseInt(require.apply("numinputs"));
        int oscListenPort = Integer.parseInt(require.apply("osclistenport"));

        SwitcherServer server = new SwitcherServer(
                casparServer,
                programChannel,
                multiviewChannel,
                multiview,
                numInputs,
                oscListenPort);

        Semaphore shouldShutdown = new Semaphore(0);

        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            shouldShutdown.release();
        }));

        shouldShutdown.acquireUninterruptibly();
        server.close();
    }
}
