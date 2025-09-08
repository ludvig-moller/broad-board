import React, { useState, useRef, useEffect } from "react";


function DrawCanvas() {
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const ctxRef = useRef<CanvasRenderingContext2D>(null);

    const [isDrawing, setIsDrawing] = useState(false);

    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) { return; }

        canvas.width = 1000;
        canvas.height = 600;

        ctxRef.current = canvas.getContext("2d");

        const ctx = ctxRef.current;
        if (!ctx) { return; }

        ctx.fillStyle = "white";
        ctx.fillRect(0, 0, canvas.width, canvas.height);
    }, []);

    const startDrawing = (e: React.MouseEvent) => {
        setIsDrawing(true);
        
        const canvas = canvasRef.current;
        const ctx = ctxRef.current;

        if (!canvas || !ctx) { return; }

        ctx.beginPath();
        ctx.moveTo(e.clientX - canvas.offsetLeft, e.clientY - canvas.offsetTop);

        ctx.lineTo(e.clientX - canvas.offsetLeft, e.clientY - canvas.offsetTop);
        ctx.strokeStyle = "black";
        ctx.lineWidth = 5;
        ctx.lineCap = "round";
        ctx.lineJoin = "round";
        ctx.stroke();

        e.preventDefault();
    }

    const stopDrawing = (e: React.MouseEvent) => {
        const ctx = ctxRef.current;

        if (!isDrawing || !ctx ) { return; }

        setIsDrawing(false);

        ctx.stroke();
        ctx.closePath();
        e.preventDefault();
    }

    const draw = (e: React.MouseEvent) => {
        const canvas = canvasRef.current;
        const ctx = ctxRef.current;

        if (!isDrawing || !canvas || !ctx ) { return }

        ctx.lineTo(e.clientX - canvas.offsetLeft, e.clientY - canvas.offsetTop);

        ctx.strokeStyle = "black";
        ctx.lineWidth = 5;
        ctx.lineCap = "round";
        ctx.lineJoin = "round";
        ctx.stroke();
    }

    return (
        <div>
            <canvas
                ref={canvasRef}
                onMouseDown={startDrawing}
                onMouseUp={stopDrawing}
                onMouseOut={stopDrawing}
                onMouseMove={draw}
            />
        </div>
    );
}

export default DrawCanvas;
