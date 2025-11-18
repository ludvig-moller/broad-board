import React, { useState, useRef, useEffect } from "react";
import { v4 as uuidv4 } from "uuid";

import { useDrawContext } from "../context/DrawContext";
import type { Point, Stroke } from "../types/stroke";

export default function DrawCanvas() {
    const { userId, drawMode, strokes, color, lineWidth, addStroke, addPointToStroke } = useDrawContext();
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const [currentStrokeId, setCurrentStrokeId] = useState<string | null>(null);

    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        canvas.width = 1280;
        canvas.height = 720;
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

    const drawStart = (e: React.MouseEvent | React.TouchEvent) => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const point = getScaledCoordinates(e, canvas);
        if (!point) return;

        const id = uuidv4();

        const stroke: Stroke = {
            id,
            userId,
            color,
            lineWidth,
            points: [point],
        };

        if (drawMode === "erase") {
            stroke.color = "white";
        }

        addStroke(stroke);
        setCurrentStrokeId(id);
    }

    const drawMove = (e: React.MouseEvent | React.TouchEvent) => {
        const canvas = canvasRef.current;
        if (!canvas || !currentStrokeId) return;

        const point = getScaledCoordinates(e, canvas);
        if (!point) return;

        addPointToStroke(currentStrokeId, point);
    }

    const drawEnd = () => {
        if (currentStrokeId) {
            setCurrentStrokeId(null);
        }
    }

    return (
        <div className="draw-canvas-container">
            <canvas className="draw-canvas"
                ref={canvasRef}
                onMouseDown={drawStart}
                onTouchStart={drawStart}
                onMouseMove={drawMove}
                onTouchMove={drawMove}
                onMouseUp={drawEnd}
                onTouchEnd={drawEnd}
                onMouseOut={(e) => { drawMove(e); drawEnd(); }}
            />
        </div>
    );
}

function getScaledCoordinates(e: React.MouseEvent | React.TouchEvent, canvas: HTMLCanvasElement): Point | null {
    const rect = e.currentTarget.getBoundingClientRect();

    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    if ("touches" in e && e.touches.length > 0) {
        return {
            x: (e.touches[0].clientX - rect.left) * scaleX,
            y: (e.touches[0].clientY - rect.top) * scaleY,
        }
    } else if ("clientX" in e) {
        return {
            x: (e.clientX - rect.left) * scaleX,
            y: (e.clientY - rect.top) * scaleY,
        }
    }

    return null;
}
