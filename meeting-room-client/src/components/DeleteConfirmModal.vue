<script setup>
const props = defineProps({
  show: { type: Boolean, required: true },
  title: { type: String, required: true },
  description: { type: String, required: true },
  confirmLabel: { type: String, default: 'Delete' },
  cancelLabel: { type: String, default: 'Cancel' },
  danger: { type: Boolean, default: true }
})
const emit = defineEmits(['confirm', 'cancel'])
</script>

<template>
  <div v-if="show" class="modal-backdrop" role="dialog" aria-modal="true">
    <div class="modal">
      <h3>{{ title }}</h3>
      <p>{{ description }}</p>
      <div class="modal-actions">
        <button :class="{ danger }" @click="$emit('confirm')">{{ confirmLabel }}</button>
        <button @click="$emit('cancel')">{{ cancelLabel }}</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.5);
  padding: 1rem;
  z-index: 50;
}
.modal {
  background: black;
  border-radius: 0.5rem;
  padding: 1.5rem;
  max-width: 90%;
  width: 400px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 1rem;
}
.modal button {
  background: none;
  border: 1px solid #0066cc;
  color: #0066cc;
  padding: 0.5rem 0.9rem;
  border-radius: 0.25rem;
  cursor: pointer;
}
.modal button:hover {
  background: rgba(0, 102, 204, 0.08);
}
.modal button.danger {
  border-color: #c92a2a;
  color: #c92a2a;
}
.modal button.danger:hover {
  background: rgba(201, 42, 42, 0.1);
}
</style>
