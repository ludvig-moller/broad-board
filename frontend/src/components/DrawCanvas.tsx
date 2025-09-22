import React, { useState, useRef, useEffect } from "react";
import { useDrawContext } from "../context/DrawContext";
import type { Stroke } from "../types/stroke";
import { v4 as uuidv4 } from "uuid";

function DrawCanvas() {
    const { strokes, addStroke, addPointToStroke } = useDrawContext();
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const [currentStrokeId, setCurrentStrokeId] = useState<string | null>(null);

    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        canvas.width = 1000;
        canvas.height = 600;
    }, []);

    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const ctx = canvas.getContext("2d");
        if (!ctx) return;

        ctx.clearRect(0, 0, canvas.width, canvas.height);

        strokes.forEach((stroke: Stroke) => {
            if (stroke.points.length <= 0) return;

            ctx.beginPath();
            ctx.strokeStyle = stroke.color;
            ctx.lineWidth = stroke.lineWidth;
            ctx.lineCap = "round";
            ctx.lineJoin = "round";

            ctx.moveTo(stroke.points[0].x, stroke.points[0].y);

            if (stroke.points.length <= 1) {
                ctx.lineTo(stroke.points[0].x, stroke.points[0].y)
            } else {
                for (let i = 1; i < stroke.points.length; i++) {
                    ctx.lineTo(stroke.points[i].x, stroke.points[i].y);
                }
            }

            ctx.stroke();
        });
    }, [strokes]);

    const startDrawing = (e: React.MouseEvent) => {
        const id = uuidv4();
        const point = { x: e.nativeEvent.offsetX, y: e.nativeEvent.offsetY };

        const stroke: Stroke = {
            id,
            color: "black",
            lineWidth: 2,
            points: [point],
        }

        addStroke(stroke);
        setCurrentStrokeId(id);
    };

    const draw = (e: React.MouseEvent) => {
        if (!currentStrokeId) return;

        const point = { x: e.nativeEvent.offsetX, y: e.nativeEvent.offsetY };
        addPointToStroke(currentStrokeId, point);
    };

    const mouseOut = (e: React.MouseEvent) => {
        if (!currentStrokeId) return;

        const point = { x: e.nativeEvent.offsetX, y: e.nativeEvent.offsetY };
        addPointToStroke(currentStrokeId, point);

        stopDrawing();
    };

    const stopDrawing = () => {
        if (currentStrokeId) {
            setCurrentStrokeId(null);
        }
    };

    return (
        <div>
            <canvas
                ref={canvasRef}
                onMouseDown={startDrawing}
                onMouseUp={stopDrawing}
                onMouseOut={mouseOut}
                onMouseMove={draw}
            />
        </div>
    );
}

export default DrawCanvas;
