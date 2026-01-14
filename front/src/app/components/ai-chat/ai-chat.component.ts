import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiService, AiMessage } from '../../services/ai.service';

@Component({
  standalone: true,
  selector: 'ai-chat',
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-chat.component.html',
  styleUrls: ['./ai-chat.component.css']
})
export class AiChatComponent {
  open = false;
  sending = false;
  input = '';
  messages: AiMessage[] = [
    { role: 'assistant', content: 'Hola, soy el asistente de Ventify. Pregunta sobre inventario o ventas.' }
  ];

  constructor(private ai: AiService) {}

  toggle() {
    this.open = !this.open;
  }

  send() {
    if (!this.input.trim()) return;
    const userMessage: AiMessage = { role: 'user', content: this.input.trim() };
    this.messages.push(userMessage);
    this.input = '';
    this.sending = true;
    this.ai.chat({ messages: this.messages }).subscribe({
      next: (resp) => {
        this.messages.push({ role: 'assistant', content: resp.message });
        this.sending = false;
      },
      error: () => {
        this.messages.push({ role: 'assistant', content: 'No pude responder ahora.' });
        this.sending = false;
      }
    });
  }
}
