package com.google.mediapipe.examples.poselandmarker

import com.google.mediapipe.tasks.vision.poselandmarker.PoseLandmarkerResult
import kotlinx.serialization.Serializable
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.net.DatagramPacket
import java.net.DatagramSocket
import java.net.InetAddress
import java.net.NetworkInterface
import java.net.SocketTimeoutException




class UDPSender(ipAddress: String?, private var port: Int) {

    @Serializable
    class SerializableCoords3D(val x: Float, val y: Float, val z: Float)

    @Serializable
    class SerializableLandmarks(val landmarks: List<SerializableCoords3D>)

    private var udpSocket = DatagramSocket(47777)
    private var ipAddress: String?

    init {
        this.ipAddress = ipAddress
        if (ipAddress == null) detectIp()
    }

    private fun detectIp() {
        // Converting Hex string to ByteArray
        fun String.decodeHex(): ByteArray {
            check(length % 2 == 0) { "Must have an even length" }

            return chunked(2)
                .map { it.toInt(16).toByte() }
                .toByteArray()
        }

        val thread = Thread {
            udpSocket.setSoTimeout(1000);

            do {
                // Broadcast message with special code for Unity Mirror Network Discovery
                val buf = "c183150125d73bc1".decodeHex()
                val packet = DatagramPacket(buf, buf.size, InetAddress.getByName("255.255.255.255"), 47777)
                udpSocket.send(packet)

                // Try to receive response message for specified time (if Ip not received - send packet again)
                do {
                    try {
                        udpSocket.receive(packet)
                        if (NetworkInterface.getByInetAddress(packet.address) == null) {
                            this.ipAddress = packet.address.hostAddress
                        }
                    } catch (e: SocketTimeoutException) {
                        break
                    }
                } while (this.ipAddress == null)
            } while (this.ipAddress == null)
        }
        thread.start()
    }

    fun sendResults(poseLandmarkerResults: PoseLandmarkerResult) {
        /* SENDING RESULTS TO SERVER */
        if (poseLandmarkerResults.worldLandmarks().isNotEmpty() && ipAddress != null) {
            println("Sending landmarks")

            val thread = Thread {
                try {
                    // Serialize landmarks into JSON format viz. https://kotlinlang.org/docs/serialization.html#example-json-serialization
                    val coords3DArray = poseLandmarkerResults.worldLandmarks().first().map {
                        SerializableCoords3D(it.x(), it.y(), it.z())
                    }

                    val landmarks = SerializableLandmarks(coords3DArray)
                    val serializedLandmarks = Json.encodeToString(landmarks)

                    val buf = serializedLandmarks.toByteArray()
                    val packet = DatagramPacket(buf, buf.size, InetAddress.getByName(ipAddress), port)
                    udpSocket.send(packet)
                } catch (e: Exception) {
                    e.printStackTrace()
                }
            }
            thread.start()
        }
    }
}

