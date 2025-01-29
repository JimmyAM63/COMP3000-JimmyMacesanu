import { Component } from '@angular/core';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent {
  messages: { text: string, sender: string, timestamp: Date }[] = [];
  newMessage: string = '';

  constructor(public authService: AuthService) {}

  sendMessage() {
    if (this.newMessage.trim() !== '') {
      const senderName = this.authService.getAuthenticatedUserName(); // Retrieve the authenticated user's name
      const newMessageObj = {
        text: this.newMessage,
        sender: senderName,
        timestamp: new Date()
      };
      this.messages.push(newMessageObj);
      this.newMessage = ''; // Clear the input field after sending message
    }
  }
}