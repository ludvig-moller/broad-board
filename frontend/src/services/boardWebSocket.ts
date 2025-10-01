import type { Stroke, Point } from "../types/stroke";

export interface BoardMessage {
    type: "init" | "addStroke" | "addPointToStroke" | "error";
    errorMessage?: string;
    strokes?: Stroke[];
    stroke?: Stroke;
    strokeId?: string;
    point?: Point;
}

export interface BoardWebSocketCallbacks {
    onStrokeAdded?: (stroke: Stroke) => void;
    onPointAddedToStroke?: (strokeId: string, point: Point) => void;
}

export class BoardWebSocket {
    private ws: WebSocket | null = null;
    private boardId: string;
    private callbacks: BoardWebSocketCallbacks;

    constructor(boardId: string, callbacks: BoardWebSocketCallbacks = {}) {
        this.boardId = boardId;
        this.callbacks = callbacks;
    }

    connect(): void {
        const wsUrl = import.meta.env.VITE_BACKEND_WEBSOCKET_URL;
        console.log(wsUrl);
        const fullUrl = `${wsUrl}?boardId=${encodeURIComponent(this.boardId)}`;

        try {
            this.ws = new WebSocket(fullUrl);
            this.setupEventListeners();
        } catch (err) {
            console.warn("Failed to create WebSocket connection: ", err);
        }
    }

    private setupEventListeners(): void {
        if (!this.ws) return;

        this.ws.onopen = () => {
            console.log("Open");
        }

        this.ws.onmessage = (e) => {
            try {
                const message: BoardMessage = JSON.parse(e.data);
                this.handleMessage(message);
            } catch (err) {
                console.warn("Failed to parse WebSocket message: ", err);
            }
        }

        this.ws.onclose = () => {
            console.log("Close");
        }
    }

    private handleMessage(message: BoardMessage): void {
        switch (message.type) {
            case "init":
                if (!message.strokes) {
                    console.warn("Recived board message type init without strokes.");
                    return;
                }

                message.strokes.forEach(stroke => {
                    this.callbacks.onStrokeAdded?.(stroke);
                });
                break;

            case "addStroke":
                if (!message.stroke) {
                    console.warn("Recived board message type addStroke without a stroke.");
                    return;
                }

                this.callbacks.onStrokeAdded?.(message.stroke);
                break;

            case "addPointToStroke":
                if (!message.strokeId || !message.point) {
                    console.warn("Recived board message type addPointToStroke without strokeId or point.");
                    return;
                }

                this.callbacks.onPointAddedToStroke?.(message.strokeId, message.point);
                break;

            case "error":
                if (!message.errorMessage) {
                    console.warn("Recived board message type error without errorMessage.");
                    return;
                }

                console.log(message.errorMessage);
                break;

            default:
                console.warn(`Recived board message with unkown type. Type: ${message.type}`);
                break;
        }
    }

    sendAddStroke(stroke: Stroke): void {
        const message: BoardMessage = {
            type: "addStroke",
            stroke,
        };

        this.send(message);
    }

    sendAddPointToStroke(strokeId: string, point: Point): void {
        const message: BoardMessage = {
            type: "addPointToStroke",
            strokeId,
            point,
        };

        this.send(message);
    }

    private send(message: BoardMessage): void {
        if (this.ws && this.ws.readyState === WebSocket.OPEN) {
            this.ws.send(JSON.stringify(message));
        }
    }

    disconnect(): void {
        console.log("disconnecting");

        if (this.ws) {
            this.ws.close(1000, "User disconnected");
            this.ws = null;
        }
    }
}
