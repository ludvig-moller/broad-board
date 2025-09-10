import React, { useState, useRef, useEffect } from "react";

interface Props {
    strokeColor: string;
    lineWidth: number;
    setClearCanvas: React.Dispatch<React.SetStateAction<() => void>>;
    setUndoLastCanvasUpdate: React.Dispatch<React.SetStateAction<() => void>>;
}

function DrawCanvas(props: Props) {
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const ctxRef = useRef<CanvasRenderingContext2D>(null);

    const [isDrawing, setIsDrawing] = useState(false);
    const [restore, setRestore] = useState<ImageData[]>([]);

    const strokeColor = props.strokeColor;
    const lineWidth = props.lineWidth;

    useEffect(() => {
        const canvas = canvasRef.current;

        if (!canvas) { return; }

        canvas.width = 1000;
        canvas.height = 600;

        ctxRef.current = canvas.getContext("2d");
    }, []);

    useEffect(() => {
        props.setClearCanvas(() => clear)
        props.setUndoLastCanvasUpdate(() => undoLast)
    }, [restore])

    const startDrawing = (e: React.MouseEvent) => {
        setIsDrawing(true);
        
        const canvas = canvasRef.current;
        const ctx = ctxRef.current;

        if (!canvas || !ctx) { return; }

        ctx.beginPath();
        ctx.moveTo(e.clientX - canvas.offsetLeft, e.clientY - canvas.offsetTop);

        ctx.lineTo(e.clientX - canvas.offsetLeft, e.clientY - canvas.offsetTop);
        ctx.strokeStyle = strokeColor;
        ctx.lineWidth = lineWidth;
        ctx.lineCap = "round";
        ctx.lineJoin = "round";
        ctx.stroke();

        e.preventDefault();
    }

    const stopDrawing = (e: React.MouseEvent) => {
        const canvas = canvasRef.current;
        const ctx = ctxRef.current;

        if (!isDrawing || !ctx || !canvas) { return; }

        setIsDrawing(false);

        ctx.stroke();
        ctx.closePath();
        e.preventDefault();

        setRestore([...restore, ctx.getImageData(0, 0, canvas.width, canvas.height)])
    }

    const draw = (e: React.MouseEvent) => {
        const canvas = canvasRef.current;
        const ctx = ctxRef.current;

        if (!isDrawing || !canvas || !ctx ) { return; }

        ctx.lineTo(e.clientX - canvas.offsetLeft, e.clientY - canvas.offsetTop);

        ctx.strokeStyle = strokeColor;
        ctx.lineWidth = lineWidth;
        ctx.lineCap = "round";
        ctx.lineJoin = "round";
        ctx.stroke();
    }

    const clear = () => {
        const canvas = canvasRef.current;
        const ctx = ctxRef.current;

        if (!canvas || !ctx ) { return; }

        ctx.fillStyle = "white";
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        setRestore([]);
    }

    const undoLast = () => {
        const ctx = ctxRef.current;

        if (!ctx || restore.length == 0) { return; }

        if (restore.length == 1) {
            clear();
        } else {
            ctx.putImageData(restore[restore.length-2], 0, 0);
            setRestore(restore.splice(0, restore.length-1));
        }
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
